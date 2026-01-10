const divTag = 'div'
const pTag = 'p'
const liTag = 'li'
const blocksTagsLower = [pTag, liTag, divTag]

export function unwrapList(listEl: HTMLOListElement | HTMLUListElement) {
  const parent = listEl.parentNode;
  if (!parent) return;


  while (listEl.firstChild) {
    const li = listEl.firstChild as HTMLElement;
    const p = document.createElement(divTag);


    while (li.firstChild) p.appendChild(li.firstChild);
    parent.insertBefore(p, listEl);
    listEl.removeChild(li);
  }

  parent.removeChild(listEl);
  parent.normalize();
}

export function findParentList(node: Node | null, editor: HTMLElement | null): HTMLOListElement | HTMLUListElement | null {
  while (node && node !== editor) {
    if (node instanceof HTMLOListElement || node instanceof HTMLUListElement) {
      return node;
    }
    node = node.parentNode;
  }
  return null;
}

export function wrapInList(
  range: Range,
  type: "ol" | "ul",
  editorRef: { value: HTMLElement | null }
) {
  if (!range || !editorRef.value) return;

  // Detect blocks inside selection
  const fragment = range.cloneContents();
  const blocks = fragment.querySelectorAll(blocksTagsLower.join(','));

  const list = document.createElement(type);
  list.style.margin = "0.5rem 0";
  list.style.paddingLeft = "1.5rem";

  if (blocks.length > 1) {
    // MULTI BLOCK SELECTION
    const extracted = range.extractContents();

    // Only use the blocks in order
    extracted.querySelectorAll(blocksTagsLower.join(',')).forEach(block => {
      const li = document.createElement(liTag);
      while (block.firstChild) li.appendChild(block.firstChild);
      list.appendChild(li);
    });
  } else {
    // SINGLE BLOCK → wrap whole selection into 1 li
    const li = document.createElement(liTag);
    li.appendChild(range.extractContents());
    list.appendChild(li);
  }

  // Insert the list
  range.insertNode(list);
  cleanUp(editorRef);

  // Position caret
  const last = list.lastChild;
  if (last) {
    const newRange = document.createRange();
    newRange.selectNodeContents(last);
    newRange.collapse(false);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(newRange);
  }
}



