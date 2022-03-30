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
    level: number;
    takenAt: Date;
}