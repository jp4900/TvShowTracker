import { Link } from "react-router-dom";
import { Heart, Star, Calendar } from "lucide-react";
import { useAddFavorite, useRemoveFavorite } from "../hooks/useTvShows";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import type { TvShowListItem } from "../types";
import { format } from "date-fns";
import { motion } from "framer-motion";

interface TvShowCardProps {
  show: TvShowListItem;
}

const TvShowCard = ({ show }: TvShowCardProps) => {
  const addFavorite = useAddFavorite();
  const removeFavorite = useRemoveFavorite();

  const handleFavoriteToggle = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();

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

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      whileHover={{ y: -8 }}
    >
      <Link to={`/show/${show.id}`}>
        <Card className="group overflow-hidden border-muted hover:border-primary/50 transition-all duration-300 hover:shadow-2xl hover:shadow-primary/10">
          <div className="relative aspect-[2/3] overflow-hidden bg-muted">
            {show.posterUrl ? (
              <img
                src={show.posterUrl}
                alt={show.title}
                className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-110"
                loading="lazy"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center text-6xl">
                🎬
              </div>
            )}

            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

            <Button
              size="icon"
              variant={show.isFavorite ? "default" : "secondary"}
              className={`absolute top-2 right-2 h-8 w-8 rounded-full transition-all ${
                show.isFavorite
                  ? "bg-red-500 hover:bg-red-600 text-white"
                  : "bg-background/80 backdrop-blur-sm hover:bg-background"
              }`}
              onClick={handleFavoriteToggle}
              disabled={addFavorite.isPending || removeFavorite.isPending}
            >
              <Heart
                className={`h-4 w-4 ${show.isFavorite ? "fill-current" : ""}`}
              />
            </Button>

            <div className="absolute top-2 left-2 flex items-center space-x-1 rounded-full bg-background/80 backdrop-blur-sm px-2 py-1">
              <Star className="h-3 w-3 fill-amber-400 text-amber-400" />
              <span className="text-xs font-semibold">
                {show.rating.toFixed(1)}
              </span>
            </div>

            <div className="absolute bottom-0 left-0 right-0 p-4 transform translate-y-full group-hover:translate-y-0 transition-transform duration-300">
              <div className="flex flex-wrap gap-1">
                {show.genres.slice(0, 3).map((genre) => (
                  <Badge key={genre} variant="secondary" className="text-xs">
                    {genre}
                  </Badge>
                ))}
              </div>
            </div>
          </div>

          <div className="p-4 space-y-2">
            <h3 className="font-semibold line-clamp-2 group-hover:text-primary transition-colors">
              {show.title}
            </h3>

            {show.releaseDate && (
              <div className="flex items-center space-x-2 text-sm text-muted-foreground">
                <Calendar className="h-3 w-3" />
                <span>{format(new Date(show.releaseDate), "yyyy")}</span>
              </div>
            )}
          </div>
        </Card>
      </Link>
    </motion.div>
  );
};

export default TvShowCard;
