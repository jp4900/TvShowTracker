import axios, { AxiosError } from "axios";
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  TvShow,
  TvShowListItem,
  PaginatedResponse,
  Episode,
  Actor,
  Recommendation,
  ApiError,
  ActorListItem,
} from "../types";

const API_BASE_URL = import.meta.env.VITE_API_URL;

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiError>) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && originalRequest) {
      const refreshToken = localStorage.getItem("refreshToken");

      if (refreshToken) {
        try {
          const response = await axios.post<AuthResponse>(
            `${API_BASE_URL}/auth/refresh`,
            { refreshToken }
          );

          const { accessToken, refreshToken: newRefreshToken } = response.data;

          localStorage.setItem("accessToken", accessToken);
          localStorage.setItem("refreshToken", newRefreshToken);

          // Retry original request
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          }
          return api(originalRequest);
        } catch (refreshError) {
          // Refresh failed, logout
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refreshToken");
          window.location.href = "/login";
          return Promise.reject(refreshError);
        }
      }
    }

    return Promise.reject(error);
  }
);

// ==========================================
// AUTH
// ==========================================

export const authApi = {
  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/auth/register", data);
    return response.data;
  },

  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>("/auth/login", data);
    return response.data;
  },

  logout: async (refreshToken: string): Promise<void> => {
    await api.post("/auth/logout", { refreshToken });
  },

  getCurrentUser: async () => {
    const response = await api.get("/auth/me");
    return response.data;
  },
};

// ==========================================
// TV SHOWS
// ==========================================

export const tvShowsApi = {
  getAll: async (params?: {
    pageNumber?: number;
    pageSize?: number;
    genreId?: string;
    type?: string;
    sortBy?: string;
    sortDesc?: boolean;
  }): Promise<PaginatedResponse<TvShowListItem>> => {
    const response = await api.get<PaginatedResponse<TvShowListItem>>(
      "/tvshows",
      { params }
    );
    return response.data;
  },

  getById: async (id: string): Promise<TvShow> => {
    const response = await api.get<TvShow>(`/tvshows/${id}`);
    return response.data;
  },

  getEpisodes: async (id: string, season?: number): Promise<Episode[]> => {
    const response = await api.get<Episode[]>(`/tvshows/${id}/episodes`, {
      params: { season },
    });
    return response.data;
  },

  getActors: async (id: string): Promise<Actor[]> => {
    const response = await api.get<Actor[]>(`/tvshows/${id}/actors`);
    return response.data;
  },

  search: async (
    query: string,
    pageNumber = 1,
    pageSize = 20
  ): Promise<PaginatedResponse<TvShowListItem>> => {
    const response = await api.get<PaginatedResponse<TvShowListItem>>(
      "/tvshows/search",
      {
        params: { query, pageNumber, pageSize },
      }
    );
    return response.data;
  },
};

// ==========================================
// ACTORS
// ==========================================

export const actorsApi = {
  getAll: async (params?: {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    sortBy?: string;
    sortDesc?: boolean;
  }): Promise<PaginatedResponse<ActorListItem>> => {
    const response = await api.get<PaginatedResponse<ActorListItem>>(
      "/actors",
      { params }
    );
    return response.data;
  },

  getById: async (id: string): Promise<Actor> => {
    const response = await api.get<Actor>(`/actors/${id}`);
    return response.data;
  },

  getTvShows: async (id: string) => {
    const response = await api.get(`/actors/${id}/tvshows`);
    return response.data;
  },
};

// ==========================================
// FAVORITES
// ==========================================

export const favoritesApi = {
  getAll: async (
    pageNumber = 1,
    pageSize = 20
  ): Promise<PaginatedResponse<TvShowListItem>> => {
    const response = await api.get<PaginatedResponse<TvShowListItem>>(
      "/favorites",
      {
        params: { pageNumber, pageSize },
      }
    );
    return response.data;
  },

  add: async (tvShowId: string): Promise<void> => {
    await api.post(`/favorites/${tvShowId}`);
  },

  remove: async (tvShowId: string): Promise<void> => {
    await api.delete(`/favorites/${tvShowId}`);
  },

  checkStatus: async (tvShowId: string): Promise<{ isFavorite: boolean }> => {
    const response = await api.get<{ isFavorite: boolean }>(
      `/favorites/${tvShowId}/status`
    );
    return response.data;
  },
};

// ==========================================
// RECOMMENDATIONS
// ==========================================

export const recommendationsApi = {
  get: async (count = 10): Promise<Recommendation[]> => {
    const response = await api.get<Recommendation[]>("/recommendations", {
      params: { count },
    });
    return response.data;
  },

  trainModel: async (): Promise<void> => {
    await api.post("/recommendations/train");
  },
};

// ==========================================
// EXPORT
// ==========================================

export const exportApi = {
  myDataCsv: async (): Promise<Blob> => {
    const response = await api.get("/export/my-data/csv", {
      responseType: "blob",
    });
    return response.data;
  },

  myDataPdf: async (): Promise<Blob> => {
    const response = await api.get("/export/my-data/pdf", {
      responseType: "blob",
    });
    return response.data;
  },

  favoritesCsv: async (): Promise<Blob> => {
    const response = await api.get("/export/favorites/csv", {
      responseType: "blob",
    });
    return response.data;
  },

  favoritesPdf: async (): Promise<Blob> => {
    const response = await api.get("/export/favorites/pdf", {
      responseType: "blob",
    });
    return response.data;
  },
};

export default api;
