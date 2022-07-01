export interface Notification {
    notifyAtUtc: Date;
    text: string;
    parent?: string;
}

export interface EventTag {
    eventId: number;
    name: string;
}

export interface CreateEventCommand {
    event: Event,
    notification?: Notification
}

export interface Event {
    id: number;
    text: string;
    occurredAtUtc: Date;
    endedAtUtc?: Date | undefined;
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