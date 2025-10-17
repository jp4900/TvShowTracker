import { useState } from "react";
import { useFavorites } from "../../hooks/useTvShows";
import TvShowCard from "../../components/TvShowCard";
import Layout from "../../components/Layout";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Heart, ChevronLeft, ChevronRight, Sparkles } from "lucide-react";
import { motion } from "framer-motion";
import { Link } from "react-router-dom";

const FavoritesPage = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const { data: favorites, isLoading } = useFavorites(currentPage, 20);

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  return (
    <Layout>
      <div className="space-y-8">
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          className="space-y-4"
        >
          <div className="flex items-center space-x-3">
            <div className="p-3 rounded-full bg-red-500/20">
              <Heart className="h-8 w-8 text-red-500 fill-current" />
            </div>
            <div>
              <h1 className="text-4xl font-bold text-foreground">
                My Favorites
              </h1>
              <p className="text-muted-foreground">
                Shows you&apos;ve marked as favorites
              </p>
            </div>
          </div>

          {favorites && favorites.totalCount > 0 && (
            <div className="flex items-center space-x-4 text-sm">
              <div className="flex items-center space-x-2">
                <div className="w-2 h-2 rounded-full bg-primary animate-pulse" />
                <span className="text-muted-foreground">
                  <span className="font-semibold text-foreground">
                    {favorites.totalCount}{" "}
                  </span>
                  shows favorited
                </span>
              </div>
            </div>
          )}
        </motion.div>

        {isLoading && (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {[...Array(20)].map((_, i) => (
              <div key={i} className="space-y-3">
                <Skeleton className="aspect-[2/3] w-full rounded-xl" />
                <Skeleton className="h-4 w-3/4" />
                <Skeleton className="h-3 w-1/2" />
              </div>
            ))}
          </div>
        )}

        {!isLoading && favorites && favorites.items.length > 0 && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: 0.1 }}
              className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6"
            >
              {favorites.items.map((show, index) => (
                <motion.div
                  key={show.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.05 }}
                >
                  <TvShowCard show={show} />
                </motion.div>
              ))}
            </motion.div>

            {favorites.totalPages > 1 && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex items-center justify-center gap-4"
              >
                <Button
                  variant="outline"
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={!favorites.hasPreviousPage}
                >
                  <ChevronLeft className="mr-2 h-4 w-4" />
                  Previous
                </Button>

                <div className="flex items-center gap-2">
                  {[...Array(Math.min(5, favorites.totalPages))].map((_, i) => {
                    let pageNum;
                    if (favorites.totalPages <= 5) {
                      pageNum = i + 1;
                    } else if (currentPage <= 3) {
                      pageNum = i + 1;
                    } else if (currentPage >= favorites.totalPages - 2) {
                      pageNum = favorites.totalPages - 4 + i;
                    } else {
                      pageNum = currentPage - 2 + i;
                    }

                    if (pageNum < 1 || pageNum > favorites.totalPages)
                      return null;

                    return (
                      <Button
                        key={pageNum}
                        variant={
                          currentPage === pageNum ? "default" : "outline"
                        }
                        size="icon"
                        onClick={() => handlePageChange(pageNum)}
                      >
                        {pageNum}
                      </Button>
                    );
                  })}
                </div>

                <Button
                  variant="outline"
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={!favorites.hasNextPage}
                >
                  Next
                  <ChevronRight className="ml-2 h-4 w-4" />
                </Button>
              </motion.div>
            )}
          </>
        )}

        {!isLoading && favorites && favorites.items.length === 0 && (
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center justify-center py-20 space-y-6"
          >
            <div className="relative">
              <div className="absolute inset-0 bg-red-500/20 blur-3xl rounded-full" />
              <Heart className="relative h-24 w-24 text-muted-foreground" />
            </div>
            <div className="text-center space-y-2">
              <h3 className="text-2xl font-semibold">No favorites yet</h3>
              <p className="text-muted-foreground max-w-md">
                Start exploring and add shows to your favorites to see them here
              </p>
            </div>
            <Button asChild size="lg">
              <Link to="/">
                <Sparkles className="mr-2 h-5 w-5" />
                Discover Shows
              </Link>
            </Button>
          </motion.div>
        )}
      </div>
    </Layout>
  );
};

export default FavoritesPage;
