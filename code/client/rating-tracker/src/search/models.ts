export interface SearchQuery {
  keywords: string;
  targetDomain: string;
  maxResults: number;
}

export interface SearchResult {
  ranks: SearchEngineResult[];
}

export interface SearchEngineResult {
  ranks: number[];
  searchEngine: string;
}
