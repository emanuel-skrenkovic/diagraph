export interface Tag {
    name: string
}

export interface Event {
    id: number;
    text: string;
    occurredAtUtc: Date;
    tags: Tag[];
}

export interface GlucoseMeasurement {
    value: number;
    occurredAt: Date;
}