import { useParams, Link } from "react-router-dom";
import { useActor } from "../../hooks/useActors";
import Layout from "../../components/Layout";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { ArrowLeft, Calendar, MapPin, Tv, User, Star } from "lucide-react";
import { motion } from "framer-motion";
import { format } from "date-fns";

const ActorPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: actor, isLoading } = useActor(id!);

  if (isLoading) {
    return (
      <Layout>
        <div className="space-y-8">
          <Skeleton className="h-12 w-48" />
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <Skeleton className="aspect-[2/3] rounded-2xl" />
            <div className="md:col-span-2 space-y-4">
              <Skeleton className="h-8 w-3/4" />
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-2/3" />
            </div>
          </div>
        </div>
      </Layout>
    );
  }

  if (!actor) {
    return (
      <Layout>
        <div className="flex flex-col items-center justify-center py-20 space-y-4">
          <User className="h-16 w-16 text-muted-foreground" />
          <h3 className="text-2xl font-semibold">Actor not found</h3>
          <Link to="/actors">
            <Button variant="outline">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Actors
            </Button>
          </Link>
        </div>
      </Layout>
    );
  }

  const age = actor.dateOfBirth
    ? new Date().getFullYear() - new Date(actor.dateOfBirth).getFullYear()
    : null;

  return (
    <Layout>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="space-y-8"
      >
        {/* Back Button */}
        <Link to="/actors">
          <Button variant="outline" size="sm" className="text-muted-foreground">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Actors
          </Button>
        </Link>

        {/* Actor Header */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Profile Image */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.1 }}
          >
            <div className="relative aspect-[2/3] rounded-2xl overflow-hidden bg-muted shadow-2xl">
              {actor.profilePath ? (
                <img
                  src={actor.profilePath}
                  alt={actor.name}
                  className="h-full w-full object-cover"
                />
              ) : (
                <div className="flex h-full w-full items-center justify-center">
                  <User className="h-32 w-32 text-muted-foreground" />
                </div>
              )}
            </div>
          </motion.div>

          {/* Actor Info */}
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.2 }}
            className="md:col-span-2 space-y-6"
          >
            <div>
              <h1 className="text-4xl md:text-5xl font-bold mb-2 text-foreground">
                {actor.name}
              </h1>
              {actor.popularity && (
                <div className="flex items-center space-x-2">
                  <Star className="h-5 w-5 fill-amber-400 text-amber-400" />
                  <span className="text-lg font-semibold text-muted-foreground">
                    {actor.popularity.toFixed(1)} Popularity
                  </span>
                </div>
              )}
            </div>

            {/* Quick Info */}
            <div className="flex flex-wrap gap-4">
              {actor.dateOfBirth && (
                <Card>
                  <CardContent className="flex items-center space-x-2 p-4">
                    <Calendar className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Born</p>
                      <p className="font-medium">
                        {format(new Date(actor.dateOfBirth), "MMM d, yyyy")}
                        {age && ` (${age} years old)`}
                      </p>
                    </div>
                  </CardContent>
                </Card>
              )}

              {actor.placeOfBirth && (
                <Card>
                  <CardContent className="flex items-center space-x-2 p-4">
                    <MapPin className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">
                        Birthplace
                      </p>
                      <p className="font-medium">{actor.placeOfBirth}</p>
                    </div>
                  </CardContent>
                </Card>
              )}

              {actor.tvShows && actor.tvShows.length > 0 && (
                <Card>
                  <CardContent className="flex items-center space-x-2 p-4">
                    <Tv className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">TV Shows</p>
                      <p className="font-medium">{actor.tvShows.length}</p>
                    </div>
                  </CardContent>
                </Card>
              )}
            </div>

            {/* Biography */}
            {actor.biography && (
              <div className="space-y-2">
                <h2 className="text-2xl font-bold text-foreground">
                  Biography
                </h2>
                <p className="text-muted-foreground leading-relaxed">
                  {actor.biography}
                </p>
              </div>
            )}
          </motion.div>
        </div>

        {/* TV Shows */}
        {actor.tvShows && actor.tvShows.length > 0 && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
            className="space-y-6"
          >
            <div className="flex items-center justify-between">
              <h2 className="text-3xl font-bold text-foreground">Known For</h2>
              <Badge variant="secondary" className="text-lg px-4 py-1">
                {actor.tvShows.length}{" "}
                {actor.tvShows.length === 1 ? "Show" : "Shows"}
              </Badge>
            </div>

            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
              {actor.tvShows.map((tvShow) => (
                <div key={tvShow.id} className="space-y-2">
                  <Link to={`/show/${tvShow.id}`}>
                    <Card className="group overflow-hidden border-muted hover:border-primary/50 transition-all duration-300 hover:shadow-xl">
                      <div className="relative aspect-[2/3] overflow-hidden bg-muted">
                        {tvShow.posterUrl ? (
                          <img
                            src={tvShow.posterUrl}
                            alt={tvShow.title}
                            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-110"
                            loading="lazy"
                          />
                        ) : (
                          <div className="flex h-full w-full items-center justify-center text-4xl">
                            🎬
                          </div>
                        )}

                        <div className="absolute top-2 left-2 flex items-center space-x-1 rounded-full bg-background/80 backdrop-blur-sm px-2 py-1">
                          <Star className="h-3 w-3 fill-amber-400 text-amber-400" />
                          <span className="text-xs font-semibold">
                            {tvShow.rating.toFixed(1)}
                          </span>
                        </div>
                      </div>

                      <div className="p-3 space-y-1">
                        <h3 className="font-semibold text-sm line-clamp-2 group-hover:text-primary transition-colors">
                          {tvShow.title}
                        </h3>
                        {tvShow.characterName && (
                          <p className="text-xs text-muted-foreground line-clamp-1">
                            as {tvShow.characterName}
                          </p>
                        )}
                      </div>
                    </Card>
                  </Link>
                </div>
              ))}
            </div>
          </motion.div>
        )}
      </motion.div>
    </Layout>
  );
};

export default ActorPage;
