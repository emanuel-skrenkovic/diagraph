export interface Tag {
    name: string
}

export interface Event {
    id: number;
    text: string;
    occurredAtUtc: Date;
    tags: Tag[];
}