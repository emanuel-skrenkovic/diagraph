export interface EventTag {
    eventId: number;
    name: string;
}

export interface Event {
    id: number;
    text: string;
    occurredAtUtc: Date;
    tags: EventTag[];
}

export interface GlucoseMeasurement {
    level: number;
    takenAt: Date;
}

export interface ImportTemplate {
    id: number;
    name: string;
    data: { headerMappings: TemplateHeaderMapping[] };
}

export interface TemplateHeaderMapping {
    header: string;
    rules: Rule[];
    tags: EventTag[];
}

export interface Rule {
    expression: string;
}