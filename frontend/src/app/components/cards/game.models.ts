export type Game = {
  id: string;
  name: string;
  genre: string;
  display_name: string;
  display_index: number;
  release_date: Date;
  thumbnail: string;
};

export interface PlayGameResponse {
  sessionId: string;
  gameUrl: string;
  expiry: string;
}

export interface GetGamesResponse {
  games: Game[];
  last_evaluated_key?: { key: string; value: string };
}
