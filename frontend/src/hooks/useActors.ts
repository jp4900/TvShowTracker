import { useQuery } from "@tanstack/react-query";
import { actorsApi } from "../services/api";

export const useActors = (params?: {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDesc?: boolean;
}) => {
  return useQuery({
    queryKey: ["actors", params],
    queryFn: () => actorsApi.getAll(params),
  });
};

export const useActor = (id: string) => {
  return useQuery({
    queryKey: ["actor", id],
    queryFn: () => actorsApi.getById(id),
    enabled: !!id,
  });
};

export const useActorTvShows = (id: string) => {
  return useQuery({
    queryKey: ["actor-tvshows", id],
    queryFn: () => actorsApi.getTvShows(id),
    enabled: !!id,
  });
};
