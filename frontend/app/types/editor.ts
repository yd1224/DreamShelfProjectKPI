export type StyleType = 'bold' | 'italic' | 'underline' | 'strikethrough';

export type StyleConfig = {
    tags: string[];
    element: string;
    selector: string;
};

export type ToolbarOptions = {
    bold: boolean;
    italic: boolean;
    underline: boolean;
    strikethrough: boolean;
    alignment: boolean;
    orderedList: boolean;
    bulletList: boolean;
    link: boolean;
    htmlMode: boolean;
    image: boolean;
};
