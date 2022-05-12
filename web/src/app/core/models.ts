export interface User {
    id: string;
    username: string;
    discriminator: string;
    isBot: boolean;
    avatarUrl: string;
}

export interface KarmaItem {
    user: User;
    position: number;
    positive: number;
    negative: number;
    value: number;
}

export interface KarmaResult {
    data: KarmaItem[];
    page: number;
    totalItemsCount: number;
    canNext: boolean;
    canPrev: boolean;
}

export class UriBuilder {
    queryParts: { key: string, value: string }[] = [];

    constructor(private baseUrl: string) { }

    withQueryParam(key: string, value: string): UriBuilder {
        this.queryParts.push({
            key: encodeURIComponent(key),
            value: encodeURIComponent(value)
        });

        return this;
    }

    toString(): string {
        return this.baseUrl +
            (this.queryParts.length == 0 ? '' : `?${this.queryParts.map(o => `${o.key}=${o.value}`).join('&')}`);
    }
}
