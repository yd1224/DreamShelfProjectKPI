import { html as beautify } from "js-beautify";

import { isFullySelected, getSelectionRange, findStyleElement, analyzeSelection, fullyRemoveStyleElement, getSelectedBlocks, isStyleActive, findBlockContainer, processNodeForRemoval, cleanupStyleTags, partiallyRemoveStyle, getClosestStyleElement, insertEmptyStyleAtCursor, removeCollapsedCursorStyle } from "../utils/textEditorUtils";
import { unwrapList, findParentList, wrapInList } from "../utils/listsUtils";
import type { StyleType, StyleConfig } from "../types/editor";

const activeFormats = ref({
    bold: false,
    italic: false,
    underline: false,
    strikethrough: false,
    alignment: 'left',
    orderedList: false,
    bulletList: false,
    link: false,
    htmlMode: false,
})

const htmlSource = ref('')

const divTag = 'div'
const pTag = 'p'
const liTag = 'li'

const blocksTagsUpper = [pTag.toUpperCase(), liTag.toUpperCase(), divTag.toUpperCase()]
const blocksTagsLower = [pTag, liTag, divTag]

// Track current images in the editor
const currentImages = ref<Set<string>>(new Set())

export function useTextEditor(editorRef: Ref<HTMLDivElement | null>) {
    // Image deletion callback
    let onImageDeleteCallback: ((fileNames: string[]) => void) | null = null;

    // Extract fileName from image src URL
    const extractFileNameFromSrc = (src: string): string | null => {
        try {
            // Extract fileName from URL like http://localhost:5153/images/fileName.jpg
            const match = src.match(/\/images\/([^?#]+)/);
            return match?.[1] ?? null;
        } catch (error) {
            console.error('Error extracting fileName from src:', error);
            return null;
        }
    }

    // Get all current images in the editor
    const getCurrentImageFileNames = (): Set<string> => {
        const fileNames = new Set<string>();
        if (editorRef.value) {
            const images = editorRef.value.querySelectorAll('img');
            images.forEach(img => {
                const fileName = extractFileNameFromSrc(img.src);
                if (fileName) {
                    fileNames.add(fileName);
                }
            });
        }
        return fileNames;
    }

    // Initialize image tracking
    const initializeImageTracking = () => {
        currentImages.value = getCurrentImageFileNames();
    }

    // Check for deleted images and trigger callback
    const checkForDeletedImages = () => {
        const newImages = getCurrentImageFileNames();
        const deletedImages: string[] = [];

        // Find images that were in currentImages but not in newImages
        currentImages.value.forEach(fileName => {
            if (!newImages.has(fileName)) {
                deletedImages.push(fileName);
            }
        });

        // Update current images
        currentImages.value = newImages;

        // Trigger callback if images were deleted
        if (deletedImages.length > 0 && onImageDeleteCallback) {
            onImageDeleteCallback(deletedImages);
        }
    }

    // Set callback for image deletion
    const onImageDelete = (callback: (fileNames: string[]) => void) => {
        onImageDeleteCallback = callback;
    }

    // Add image to tracking
    const trackImage = (fileName: string) => {
        currentImages.value.add(fileName);
    }

    // Style configuration map
    const styleConfig = {
        bold: { tags: ['STRONG', 'B'], element: 'strong', selector: 'strong' },
        italic: { tags: ['EM', 'I'], element: 'em', selector: 'em' },
        underline: { tags: ['U'], element: 'u', selector: 'u' },
        strikethrough: { tags: ['S', 'STRIKE', 'DEL'], element: 's', selector: 's' }
    };

    function removeMultiParagraph(range: Range, config: any, editorRef: { value: HTMLElement | null }) {
        const extracted = range.extractContents();
        const fragment = document.createDocumentFragment();

        Array.from(extracted.childNodes).forEach(child => {
            fragment.appendChild(processNodeForRemoval(child, config));
        });

        range.insertNode(fragment);

        if (editorRef.value) {
            const allBlocks = editorRef.value.querySelectorAll(blocksTagsLower.join(','));
            allBlocks.forEach(block => cleanupStyleTags(block, config.selector, config.tags));
        }

        const newRange = document.createRange();
        newRange.selectNodeContents(range.commonAncestorContainer);
        newRange.collapse(false);
        restoreSelection(newRange);
    }

    function addStyleMultiParagraph(range: Range, config: any, editorRef: { value: HTMLElement | null }) {
        const extracted = range.extractContents();
        const newFragment = document.createDocumentFragment();

        const processNode = (node: Node): Node => {
            if (node.nodeType === Node.ELEMENT_NODE) {
                const el = node as HTMLElement;

                if (blocksTagsUpper.includes(el.tagName)) {
                    const clone = el.cloneNode(false) as HTMLElement;
                    const styled = document.createElement(config.element);
                    // Move children of el into styled
                    while (el.firstChild) styled.appendChild(el.firstChild);
                    clone.appendChild(styled);
                    return clone;
                }

                const styled = document.createElement(config.element);
                styled.appendChild(node.cloneNode(true));
                return styled;
            }

            if (node.nodeType === Node.TEXT_NODE && node.textContent?.trim()) {
                const wrapper = document.createElement(config.element);
                wrapper.appendChild(node.cloneNode(true));
                return wrapper;
            }

            return node.cloneNode(true);
        };

        Array.from(extracted.childNodes).forEach(child => newFragment.appendChild(processNode(child)));

        range.insertNode(newFragment);

        if (editorRef.value) {
            const allBlocks = editorRef.value.querySelectorAll(blocksTagsLower.join(','));
            allBlocks.forEach(p => cleanupStyleTags(p, config.selector, config.tags));
        }

        const newRange = document.createRange();
        newRange.selectNodeContents(range.commonAncestorContainer);
        newRange.collapse(false);
        restoreSelection(newRange);
    }

    function addStyleInline(range: Range, config: any) {
        const wrapper = document.createElement(config.element);
        wrapper.appendChild(range.extractContents());
        range.insertNode(wrapper);

        const parent = wrapper.parentNode;
        if (parent) {
            cleanupStyleTags(parent, config.selector, config.tags);
        }

        const newRange = document.createRange();
        newRange.selectNodeContents(range.commonAncestorContainer);
        newRange.collapse(false);
        restoreSelection(newRange);
    }

    function toggleTextStyle(styleType: StyleType, callback?: () => void) {
        const range = getSelectionRange();
        if (!range) return;

        const config = styleConfig[styleType];

        if (range.collapsed) {
            applyStyleOnCollapsedCursor(range, config);
        } else {
            applyStyleOnSelection(range, config);
        }

        callback?.();
    }

    function applyStyleOnCollapsedCursor(range: Range, config: StyleConfig) {
        const editor = editorRef.value;
        if (!editor) return;

        // Check if cursor is already in styled block
        const parentStyled = getClosestStyleElement(range.startContainer, config, editor);

        // If already inside → remove style
        if (parentStyled) {
            removeCollapsedCursorStyle(range, parentStyled);
            return;
        }

        // Otherwise → create new style element for typing
        insertEmptyStyleAtCursor(range, config);
    }

    function applyStyleOnSelection(range: Range, config: StyleConfig) {
        const styleElement = findStyleElement(range, editorRef.value, config);
        const { hasMultipleBlocks, allBlocksStyled } = analyzeSelection(range, config);

        if (styleElement && !hasMultipleBlocks) {
            const fullySelected = isFullySelected(styleElement, range);
            if (fullySelected) {
                fullyRemoveStyleElement(styleElement, config);
            } else {
                partiallyRemoveStyle(range, styleElement, config);
            }
            return;
        }

        if (hasMultipleBlocks && allBlocksStyled) {
            removeMultiParagraph(range, config, editorRef);
            return;
        }

        if (hasMultipleBlocks) {
            addStyleMultiParagraph(range, config, editorRef);
            return;
        }

        addStyleInline(range, config);
    }

    const handleBeforeInput = (event: InputEvent, callback?: () => void) => {
        if (!editorRef.value || !event.data) return
        event.preventDefault()
        insertText(event.data)
        // Trigger callback after text insertion to update modelValue
        if (callback) {
            callback()
        }
    }

    const handlePaste = (event: ClipboardEvent, callback?: () => void) => {
        if (!editorRef.value) return
        event.preventDefault()
        const text = event.clipboardData?.getData('text/plain')
        if (!text) return
        insertText(text)
        // Trigger callback after text insertion to update modelValue
        if (callback) {
            callback()
        }
    }

    function insertList(type: 'ol' | 'ul', callback?: () => void) {
        const range = getSelectionRange();
        if (!range || range.collapsed) return;

        const editor = editorRef.value;
        const listEl = findParentList(range.commonAncestorContainer, editor);


        // 1. Toggle off if already same type
        if (
            (type === 'ol' && listEl instanceof HTMLOListElement) ||
            (type === 'ul' && listEl instanceof HTMLUListElement)
        ) {
            unwrapList(listEl);
            return;
        }

        // 2. Convert list type (UL → OL or OL → UL)
        if (listEl instanceof HTMLOListElement || listEl instanceof HTMLUListElement) {
            const newList = document.createElement(type);
            while (listEl.firstChild) newList.appendChild(listEl.firstChild);
            listEl.replaceWith(newList);
            return;
        }

        // 3. Wrap selection into a new OL/UL
        wrapInList(range, type, editorRef);
        if (callback) {
            callback()
        }
    }

    // This function intercepts input before it is inserted into the DOM
    const insertText = (char: string) => {
        const selection = window.getSelection()
        if (!selection || !selection.rangeCount) return
        const range = selection.getRangeAt(0)

        let charToInsert = char
        if (char === '\t') charToInsert = '\u00A0\u00A0\u00A0\u00A0'
        else if (char === ' ') charToInsert = '\u00A0'

        let parentP: HTMLElement | null = null
        let currentNode: Node | null = range.startContainer

        while (currentNode && currentNode !== editorRef.value) {
            if (currentNode instanceof HTMLElement && currentNode.tagName === divTag.toUpperCase()) {
                parentP = currentNode
                break
            }
            currentNode = currentNode.parentNode
        }

        if (!parentP) {
            parentP = document.createElement(divTag)
            range.insertNode(parentP)
            range.setStart(parentP, 0)
            range.collapse(true)
        }

        const textNode = document.createTextNode(charToInsert)
        range.insertNode(textNode)

        // Move caret after the inserted node
        range.setStartAfter(textNode)
        range.collapse(true)
        selection.removeAllRanges()
        selection.addRange(range)
    }

    function toggleAlignment(alignment: "left" | "center" | "right" | "justify", callback?: () => void) {
        const range = getSelectionRange();
        if (!range) return;

        const editor = editorRef.value;
        if (!editor) return;

        const blocks = getSelectedBlocks(range, editorRef);
        if (blocks.length === 0) return;

        blocks.forEach(block => {
            if (block.style.textAlign === alignment) {
                block.style.textAlign = "";   // reset
            } else {
                block.style.textAlign = alignment;
            }
        });

        editor.normalize();
        if (callback) {
            callback()
        }
    }

    const handleKeyDown = (e: KeyboardEvent) => {
        if (e.key === 'Tab') {
            e.preventDefault();
            insertText('\t');
        }
    };

    function updateActiveFormats() {
        const range = getSelectionRange();
        if (!range) return;

        // Inline styles
        activeFormats.value.bold = isStyleActive(range, ['B', 'STRONG'], editorRef);
        activeFormats.value.italic = isStyleActive(range, ['I', 'EM'], editorRef);
        activeFormats.value.underline = isStyleActive(range, ['U'], editorRef);
        activeFormats.value.strikethrough = isStyleActive(range, ['S', 'DEL'], editorRef);

        // Block styles
        const block = findBlockContainer(range.commonAncestorContainer, editorRef.value);
        if (block) {
            activeFormats.value.alignment = block.style.textAlign || 'left';

            // Check if inside a list
            if (block.tagName === liTag.toUpperCase()) {
                const parentList = block.parentElement;
                activeFormats.value.orderedList = parentList?.tagName === 'OL';
                activeFormats.value.bulletList = parentList?.tagName === 'UL';
            } else {
                activeFormats.value.orderedList = block.tagName === 'OL';
                activeFormats.value.bulletList = block.tagName === 'UL';
            }
        } else {
            activeFormats.value.alignment = 'left';
            activeFormats.value.orderedList = false;
            activeFormats.value.bulletList = false;
        }

        // Optional: link detection
        const anchor = isStyleActive(range, ['A'], editorRef); // similar to isStyleActive

        activeFormats.value.link = !!anchor;
    }

    const insertLink = (linkUrl: string, savedSelection: Range | null, callback?: () => void) => {
        if (!linkUrl || !savedSelection) return;

        const selection = window.getSelection();
        if (!selection) return;

        // Restore previous selection
        selection.removeAllRanges();
        selection.addRange(savedSelection);

        const range = selection.getRangeAt(0);

        // Extract selected content
        const content = range.extractContents();

        // Create the link element
        const anchor = document.createElement("a");
        anchor.href = linkUrl;
        anchor.target = "_blank";
        anchor.rel = "noopener noreferrer";
        anchor.style.color = "#3b82f6";
        anchor.style.textDecoration = "underline";
        anchor.appendChild(content);

        // Insert <a> back into the DOM
        range.insertNode(anchor);

        // Move cursor after the link
        const newRange = document.createRange();
        newRange.setStartAfter(anchor);
        newRange.collapse(true);
        selection.removeAllRanges();
        selection.addRange(newRange);
        if (callback) {
            callback()
        }
    };

    function insertImageAtSelection(src: string) {
        const selection = window.getSelection();
        if (!selection || selection.rangeCount === 0) return;

        const range = selection.getRangeAt(0);

        // Create <img>
        const img = document.createElement("img");
        img.src = src;
        img.alt = "";
        img.style.maxWidth = "400px";
        img.style.display = "inline-block";
        img.style.height = "auto";

        // Insert image at caret
        range.insertNode(img);

        // Move cursor after the image
        const newRange = document.createRange();
        newRange.setStartAfter(img);
        newRange.collapse(true);

        selection.removeAllRanges();
        selection.addRange(newRange);
    }


    function toggleHtmlMode() {
        if (!activeFormats.value.htmlMode) {
            // Switching to HTML mode
            htmlSource.value = beautify(editorRef.value?.innerHTML || "", {
                indent_size: 2,
                wrap_line_length: 80,
                preserve_newlines: true
            });
            activeFormats.value.htmlMode = true;
        } else {
            // Switching back to normal mode
            if (editorRef.value) {
                editorRef.value.innerHTML = htmlSource.value;
            }
            activeFormats.value.htmlMode = false;
        }
    }

    return {
        handleBeforeInput,
        handleKeyDown,
        toggleTextStyle,
        insertList,
        toggleAlignment,
        updateActiveFormats,
        insertLink,
        insertImageAtSelection,
        activeFormats,
        toggleHtmlMode,
        htmlSource,
        handlePaste,
        initializeImageTracking,
        checkForDeletedImages,
        onImageDelete,
        trackImage,
        currentImages
    }
}