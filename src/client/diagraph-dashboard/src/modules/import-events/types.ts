export interface ImportTemplate {
    id: number;
    name: string;
    data: { headerMappings: TemplateHeaderMapping[] };
}

export interface TemplateHeaderMapping {
    header: string;
    rules: Rule[];
    tags: string[];
}

export interface Rule {
    expression: string;
}