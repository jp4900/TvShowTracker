import { useParams, Link } from "react-router-dom";
import {
  useTvShow,
  useEpisodes,
  useActors,
  useAddFavorite,
  useRemoveFavorite,
} from "../../hooks/useTvShows";
import Layout from "../../components/Layout";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Skeleton } from "@/components/ui/skeleton";
import { Heart, Star, Calendar, Tv, ArrowLeft, Users } from "lucide-react";
import { motion } from "framer-motion";
import { format } from "date-fns";
import { toast } from "sonner";
import { TrailerModal } from "@/components/TrailerModal";
import { Play } from "lucide-react";
import { useState } from "react";

const TvShowDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: show, isLoading: showLoading } = useTvShow(id!);
  const { data: episodes, isLoading: episodesLoading } = useEpisodes(id!);
  const { data: actors, isLoading: actorsLoading } = useActors(id!);
  const [showTrailer, setShowTrailer] = useState(false);

  const addFavorite = useAddFavorite();
  const removeFavorite = useRemoveFavorite();

  if (showLoading) {
    return (
      <Layout>
        <div className="space-y-8">
          <Skeleton className="h-96 w-full rounded-2xl" />
          <div className="space-y-4">
            <Skeleton className="h-8 w-3/4" />
            <Skeleton className="h-4 w-full" />
            <Skeleton className="h-4 w-full" />
          </div>
        </div>
      </Layout>
    );
  }

  if (!show) {
    return (
      <Layout>
        <div className="flex flex-col items-center justify-center py-20 space-y-4">
          <div className="text-6xl">😕</div>
          <h2 className="text-2xl font-semibold">Show not found</h2>
          <Button asChild>
            <Link to="/">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Home
            </Link>
          </Button>
        </div>
      </Layout>
    );
  }

  const handleFavoriteToggle = async () => {
    try {
      if (show.isFavorite) {
        await removeFavorite.mutateAsync(show.id);
        toast.success("Removed from favorites");
      } else {
        await addFavorite.mutateAsync(show.id);
        toast.success("Added to favorites");
      }
    } catch (error) {
      toast.error("Failed to update favorites");
    }
  };

  // Group episodes by season
  const episodesBySeason = episodes?.reduce((acc, episode) => {
    if (!acc[episode.seasonNumber]) {
      acc[episode.seasonNumber] = [];
    }
    acc[episode.seasonNumber].push(episode);
    return acc;
  }, {} as Record<number, typeof episodes>);

  return (
    <Layout>
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        className="space-y-8"
      >
        <Button variant="ghost" asChild>
          <Link to="/">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back
          </Link>
        </Button>

        <div className="relative overflow-hidden rounded-2xl">
          {show.backdropUrl && (
            <div className="absolute inset-0">
              <img
                src={show.backdropUrl}
                alt={show.title}
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-background via-background/80 to-background/40" />
            </div>
          )}

          <div className="relative z-10 p-8 md:p-12">
            <div className="flex flex-col md:flex-row gap-8">
              <div className="flex-shrink-0">
                <Card className="w-48 aspect-[2/3] overflow-hidden">
                  {show.posterUrl ? (
                    <img
                      src={show.posterUrl}
                      alt={show.title}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <div className="flex items-center justify-center w-full h-full text-6xl">
                      🎬
                    </div>
                  )}
                </Card>
              </div>

              <div className="flex-1 space-y-4">
                <div>
                  <h1 className="text-4xl md:text-5xl font-bold mb-2 text-foreground">
                    {show.title}
                  </h1>
                  <div className="flex flex-wrap items-center gap-3 text-muted-foreground">
                    {show.releaseDate && (
                      <div className="flex items-center space-x-2">
                        <Calendar className="h-4 w-4" />
                        <span>
                          {format(new Date(show.releaseDate), "yyyy")}
                        </span>
                      </div>
                    )}
                    {show.numberOfSeasons && (
                      <div className="flex items-center space-x-2">
                        <Tv className="h-4 w-4" />
                        <span>{show.numberOfSeasons} Seasons</span>
                      </div>
                    )}
                    <Badge variant="outline">{show.status}</Badge>
                    <Badge variant="outline">{show.type}</Badge>
                  </div>
                </div>

                <div className="flex items-center gap-4">
                  <div className="flex items-center space-x-2 bg-primary/20 rounded-full px-4 py-2">
                    <Star className="h-5 w-5 fill-amber-400 text-amber-400" />
                    <span className="text-lg font-semibold">
                      {show.rating.toFixed(1)}
                    </span>
                    <span className="text-sm text-muted-foreground">/ 10</span>
                  </div>

                  <Button
                    size="lg"
                    variant="outline"
                    onClick={() => setShowTrailer(true)}
                    className="gap-2"
                  >
                    <Play className="h-5 w-5" />
                    Watch Trailer
                  </Button>

                  <Button
                    size="lg"
                    variant={show.isFavorite ? "default" : "outline"}
                    onClick={handleFavoriteToggle}
                    disabled={addFavorite.isPending || removeFavorite.isPending}
                    className={
                      show.isFavorite
                        ? "bg-red-500 hover:bg-red-600 text-foreground"
                        : "text-muted-foreground"
                    }
                  >
                    <Heart
                      className={`mr-2 h-5 w-5 ${
                        show.isFavorite ? "fill-current" : ""
                      }`}
                    />
                    {show.isFavorite ? "Favorited" : "Add to Favorites"}
                  </Button>
                </div>

                <div className="flex flex-wrap gap-2">
                  {show.genres.map((genre) => (
                    <Badge key={genre} variant="secondary">
                      {genre}
                    </Badge>
                  ))}
                </div>

                <p className="text-lg text-foreground/90 max-w-3xl">
                  {show.description}
                </p>
              </div>
            </div>
          </div>
        </div>

        <Tabs defaultValue="episodes" className="w-full">
          <TabsList className="grid w-full max-w-md grid-cols-2">
            <TabsTrigger value="episodes">Episodes</TabsTrigger>
            <TabsTrigger value="cast">Cast</TabsTrigger>
          </TabsList>

          <TabsContent value="episodes" className="space-y-6">
            {episodesLoading ? (
              <div className="space-y-4">
                {[...Array(5)].map((_, i) => (
                  <Skeleton key={i} className="h-24 w-full" />
                ))}
              </div>
            ) : episodesBySeason && Object.keys(episodesBySeason).length > 0 ? (
              Object.entries(episodesBySeason)
                .sort(([a], [b]) => Number(a) - Number(b))
                .map(([season, eps]) => (
                  <div key={season} className="space-y-4">
                    <h3 className="text-2xl font-semibold text-foreground">
                      Season {season}
                    </h3>
                    <div className="grid gap-4">
                      {eps.map((episode) => (
                        <Card
                          key={episode.id}
                          className="p-4 hover:border-primary/50 transition-colors"
                        >
                          <div className="flex gap-4">
                            <div className="flex-shrink-0">
                              <div className="w-24 h-16 bg-muted rounded flex items-center justify-center text-xs font-semibold">
                                {episode.episodeNumber}
                              </div>
                            </div>
                            <div className="flex-1 space-y-1">
                              <h4 className="font-semibold">{episode.title}</h4>
                              {episode.description && (
                                <p className="text-sm text-muted-foreground line-clamp-2">
                                  {episode.description}
                                </p>
                              )}
                              <div className="flex items-center gap-4 text-xs text-muted-foreground">
                                {episode.releaseDate && (
                                  <span>
                                    {format(
                                      new Date(episode.releaseDate),
                                      "MMM d, yyyy"
                                    )}
                                  </span>
                                )}
                                {episode.length && (
                                  <span>{episode.length} min</span>
                                )}
                                {episode.rating && (
                                  <div className="flex items-center space-x-1">
                                    <Star className="h-3 w-3 fill-amber-400 text-amber-400" />
                                    <span>{episode.rating.toFixed(1)}</span>
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        </Card>
                      ))}
                    </div>
                  </div>
                ))
            ) : (
              <div className="text-center py-12 text-muted-foreground">
                No episode information available
              </div>
            )}
          </TabsContent>

          <TabsContent value="cast">
            {actorsLoading ? (
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-6">
                {[...Array(12)].map((_, i) => (
                  <div key={i} className="space-y-2">
                    <Skeleton className="aspect-[2/3] w-full rounded-lg" />
                    <Skeleton className="h-4 w-3/4" />
                  </div>
                ))}
              </div>
            ) : actors && actors.length > 0 ? (
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-6">
                {actors.map((actor) => (
                  <Link to={`/actors/${actor.id}`}>
                    <Card
                      key={actor.id}
                      className="overflow-hidden hover:border-primary/50 transition-colors"
                    >
                      <div className="aspect-[2/3] bg-muted">
                        {actor.profilePath ? (
                          <img
                            src={actor.profilePath}
                            alt={actor.name}
                            className="w-full h-full object-cover"
                          />
                        ) : (
                          <div className="flex items-center justify-center w-full h-full">
                            <Users className="h-12 w-12 text-muted-foreground" />
                          </div>
                        )}
                      </div>
                      <div className="p-3 space-y-1">
                        <p className="font-semibold text-sm line-clamp-1">
                          {actor.name}
                        </p>
                        {actor.characterName && (
                          <p className="text-xs text-muted-foreground line-clamp-1">
                            as {actor.characterName}
                          </p>
                        )}
                      </div>
                    </Card>
                  </Link>
                ))}
              </div>
            ) : (
              <div className="text-center py-12 text-muted-foreground">
                No cast information available
              </div>
            )}
          </TabsContent>
        </Tabs>
      </motion.div>
      <TrailerModal
        showTitle={show?.title || ""}
        isOpen={showTrailer}
        onClose={() => setShowTrailer(false)}
      />
    </Layout>
  );
};

export default TvShowDetailPage;
