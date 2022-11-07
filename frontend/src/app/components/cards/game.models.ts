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
  session_id: string;
  game_url: string;
  expiry: string;
}

export interface GetGamesResponse {
  games: Game[];
  last_evaluated_key?: { key: string; value: string };
}
