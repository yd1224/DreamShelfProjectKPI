import type { StyleConfig } from "../types/editor";

const divTag = 'div'
const pTag = 'p'
const liTag = 'li'

const blocksTagsUpper = [pTag.toUpperCase(), liTag.toUpperCase(), divTag.toUpperCase()]
const blocksTagsLower = [pTag, liTag, divTag]

export function isFullySelected(styleElement: HTMLElement, range: Range): boolean {
    const styleRange = document.createRange();
    styleRange.selectNodeContents(styleElement);

    return (
        range.startContainer === styleRange.startContainer &&
        range.startOffset === styleRange.startOffset &&
        range.endContainer === styleRange.endContainer &&
        range.endOffset === styleRange.endOffset
    ) || range.toString() === styleElement.textContent;
}

// Process each child node to remove styles
export const processNodeForRemoval = (node: Node, config: { tags: string[], element: string, selector: string }): Node => {
    if (node.nodeType === Node.ELEMENT_NODE) {
        const element = node as HTMLElement;

        // If it's a block element, process its children
        if (blocksTagsUpper.includes(element.tagName)) {
            const blockClone = element.cloneNode(false) as HTMLElement;

            // Remove all style elements from inside this block
            const removeStyles = (parent: Node) => {
                Array.from(parent.childNodes).forEach(child => {
                    if (child.nodeType === Node.ELEMENT_NODE) {
                        const childElement = child as HTMLElement;

                        // If it's the style we're removing, unwrap it
                        if (config.tags.includes(childElement.tagName)) {
                            while (childElement.firstChild) {
                                parent.insertBefore(childElement.firstChild, childElement);
                            }
                            parent.removeChild(childElement);
                        } else {
                            // Recursively process children
                            removeStyles(childElement);
                        }
                    }
                });
            };

            // Copy all children to the clone
            while (element.firstChild) {
                blockClone.appendChild(element.firstChild);
            }

            // Remove styles from the clone
            removeStyles(blockClone);
            blockClone.normalize();

            return blockClone;
        } else if (config.tags.includes(element.tagName)) {
            // If it's the style element itself, unwrap it
            const fragment = document.createDocumentFragment();
            while (element.firstChild) {
                fragment.appendChild(element.firstChild);
            }
            return fragment;
        }
    }
    return node.cloneNode(true);
};

// Helper function to clean up nested and adjacent tags for a specific style
export const cleanupStyleTags = (container: Node, selector: string, tags: string[]) => {
    if (!container || !(container instanceof Element)) return;

    // Remove nested tags by unwrapping inner ones
    const nestedElements = Array.from(container.querySelectorAll(`${selector} ${selector}`));

    nestedElements.forEach((innerElement) => {
        const parent = innerElement.parentElement;
        if (parent && tags.includes(parent.tagName)) {
            // Unwrap the inner element - move its children to parent
            while (innerElement.firstChild) {
                parent.insertBefore(innerElement.firstChild, innerElement);
            }
            parent.removeChild(innerElement);
        }
    });

    // Merge adjacent tags
    const allElements = Array.from(container.querySelectorAll(selector));

    allElements.forEach((element) => {
        let nextElement = element.nextElementSibling; // Use nextElementSibling instead of nextSibling

        while (nextElement && tags.includes(nextElement.tagName)) {
            // Move all children from next element to current element
            while (nextElement.firstChild) {
                element.appendChild(nextElement.firstChild);
            }
            // Remove the now-empty next element
            const toRemove = nextElement;
            nextElement = nextElement.nextElementSibling; // Move to next element
            toRemove.parentNode?.removeChild(toRemove);
        }
    });


    // Remove empty tags
    const emptyElements = Array.from(container.querySelectorAll(`${selector}:empty`));

    emptyElements.forEach((emptyElement) => {
        emptyElement.parentNode?.removeChild(emptyElement);
    });
}

export function getSelectionRange(): Range | null {
    const selection = window.getSelection();
    if (!selection || !selection.rangeCount) return null;
    return selection.getRangeAt(0);
}

export function restoreSelection(range: Range) {
    const selection = window.getSelection();
    if (!selection) return;
    selection.removeAllRanges();
    selection.addRange(range);
}

export function findStyleElement(range: Range, editor: HTMLElement | null, config: any): HTMLElement | null {
    let node: Node | null = range.commonAncestorContainer;
    while (node && node !== editor) {
        if (node instanceof HTMLElement && config.tags.includes(node.tagName)) {
            return node;
        }
        node = node.parentNode;
    }
    return null;
}

export function analyzeSelection(range: Range, config: any) {
    const fragment = range.cloneContents();
    const styled = fragment.querySelectorAll(config.selector);
    const paragraphs = fragment.querySelectorAll(divTag);
    return {
        hasMultipleBlocks: paragraphs.length > 0,
        allBlocksStyled: styled.length === paragraphs.length
    };
}

export function fullyRemoveStyleElement(styleElement: HTMLElement, config: any) {
    const parent = styleElement.parentNode;
    if (!parent) return;

    while (styleElement.firstChild) {
        parent.insertBefore(styleElement.firstChild, styleElement);
    }
    parent.removeChild(styleElement);
    parent.normalize();

    cleanupStyleTags(parent, config.selector, config.tags);
}

export function getTextOffset(styleElement: HTMLElement, node: Node, offset: number): number {
    let textOffset = 0;
    const walker = document.createTreeWalker(styleElement, NodeFilter.SHOW_TEXT);
    let currentNode: Node | null;
    while ((currentNode = walker.nextNode())) {
        if (currentNode === node) {
            return textOffset + offset;
        }
        textOffset += currentNode.textContent?.length || 0;
    }
    return textOffset;
}

export function partiallyRemoveStyle(range: Range, styleElement: HTMLElement, config: any) {
    const parent = styleElement.parentNode;
    if (!parent) return;

    const elementText = styleElement.textContent || '';
    const selectedText = range.toString();

    const start = getTextOffset(styleElement, range.startContainer, range.startOffset);
    const end = getTextOffset(styleElement, range.endContainer, range.endOffset);

    const beforeText = elementText.substring(0, start);
    const afterText = elementText.substring(end);

    const fragment = document.createDocumentFragment();

    if (beforeText) {
        const beforeElement = document.createElement(config.element);
        beforeElement.textContent = beforeText;
        fragment.appendChild(beforeElement);
    }

    const unstyled = document.createTextNode(selectedText);
    fragment.appendChild(unstyled);

    if (afterText) {
        const afterElement = document.createElement(config.element);
        afterElement.textContent = afterText;
        fragment.appendChild(afterElement);
    }

    parent.replaceChild(fragment, styleElement);
    parent.normalize();

    cleanupStyleTags(parent, config.selector, config.tags);

    // Restore caret at end of the unstyled text
    const newRange = document.createRange();
    newRange.setStart(unstyled, selectedText.length);
    newRange.collapse(true);
    restoreSelection(newRange);
}

export function findBlockContainer(node: Node | null, editor: HTMLElement | null): HTMLElement | null {
    while (node && node !== editor) {
        if (node instanceof HTMLElement) {
            const tag = node.tagName;
            if (["P", "DIV", "LI", "OL", "UL"].includes(tag)) {
                return node;
            }
        }
        node = node.parentNode;
    }
    return null;
}

export function getSelectedBlocks(range: Range, editorRef: { value: HTMLElement | null }): HTMLElement[] {
    const fragment = range.cloneContents();
    const blocks = [...fragment.querySelectorAll(blocksTagsLower.join(','))];

    // If selection is fully inside a single block and fragment finds nothing:
    if (blocks.length === 0) {
        const root = findBlockContainer(range.commonAncestorContainer, editorRef.value);
        return root ? [root] : [];
    }

    return blocks;
}

export function isStyleActive(range: Range, tagNames: string[], editorRef: { value: HTMLElement | null }): boolean {
    let node: Node | null = range.commonAncestorContainer;

    while (node && node !== editorRef.value) {
        if (node instanceof HTMLElement && tagNames.includes(node.tagName)) {
            return true;
        }
        node = node.parentNode;
    }
    return false;
}

export function getClosestAncestor(node: Node | null, root: HTMLElement, tag: string) {
    tag = tag.toUpperCase();
    while (node && node !== root) {
        if ((node as HTMLElement).nodeName === tag) return node as HTMLElement;
        node = node.parentNode;
    }
    return null;
}

export function getClosestStyleElement(node: Node | null, config: StyleConfig, root: HTMLElement) {
    while (node && node !== root) {
        if (node.nodeType === 1) {
            const el = node as HTMLElement;
            if (config.tags.includes(el.tagName)) return el;
        }
        node = node.parentNode;
    }
    return null;
}

export function insertEmptyStyleAtCursor(range: Range, config: StyleConfig) {
    const styleEl = document.createElement(config.element);

    const editor = document.querySelector('.editor-content')!;
    const directlyInRoot = range.startContainer === editor;

    if (directlyInRoot) {
        const wrapper = document.createElement('div');
        wrapper.appendChild(styleEl);
        range.insertNode(wrapper);
    } else {
        range.insertNode(styleEl);
    }

    // place cursor inside the new tag
    range.setStart(styleEl, 0);
    range.collapse(true);
    restoreSelection(range);
}

export function removeCollapsedCursorStyle(range: Range, parentStyled: HTMLElement) {
    const parent = parentStyled.parentNode!;
    const index = Array.from(parent.childNodes).indexOf(parentStyled);

    range.setStart(parent, index + 1);
    range.collapse(true);
    restoreSelection(range);
}