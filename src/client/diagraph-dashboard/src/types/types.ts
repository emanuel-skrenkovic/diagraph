export interface Tag {
    name: string
}

export interface Event {
    id: number;
    text: string;
    occurredAt: Date;
    tags: Tag[];
}

export interface GlucoseMeasurement {
    value: number;
    occurredAt: Date;
}