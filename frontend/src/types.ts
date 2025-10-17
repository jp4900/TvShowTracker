export interface User {
  id: string;
  email: string;
  lastLoginAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  dataProcessingConsent: boolean;
}

export interface TvShow {
  id: string;
  title: string;
  description: string;
  releaseDate?: string;
  endDate?: string;
  status: string;
  type: string;
  posterUrl?: string;
  backdropUrl?: string;
  rating: number;
  voteCount: number;
  numberOfSeasons?: number;
  numberOfEpisodes?: number;
  popularity: number;
  genres: string[];
  isFavorite: boolean;
}

export interface TvShowListItem {
  id: string;
  title: string;
  posterUrl?: string;
  rating: number;
  releaseDate?: string;
  genres: string[];
  isFavorite: boolean;
}

export interface Episode {
  id: string;
  title: string;
  description?: string;
  seasonNumber: number;
  episodeNumber: number;
  releaseDate?: string;
  length?: number;
  stillPath?: string;
  rating?: number;
  voteCount?: number;
}

export interface Actor {
  id: string;
  name: string;
  biography?: string;
  dateOfBirth?: string;
  placeOfBirth?: string;
  profilePath?: string;
  popularity?: number;
  characterName?: string;
  tvShows?: ActorTvShow[];
}

export interface ActorTvShow {
  id: string;
  title: string;
  posterUrl?: string;
  characterName?: string;
  rating: number;
}

export interface ActorListItem {
  id: string;
  name: string;
  profilePath?: string;
  popularity?: number;
  dateOfBirth?: string;
  placeOfBirth?: string;
  tvShowCount: number;
}

export interface Recommendation {
  id: string;
  title: string;
  posterUrl?: string;
  rating: number;
  releaseDate?: string;
  genres: string[];
  isFavorite: boolean;
  score: number;
  reason: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiError {
  message: string;
  errors?: string[];
}
