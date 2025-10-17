import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { useSearchTvShows } from "../../hooks/useTvShows";
import TvShowCard from "../../components/TvShowCard";
import Layout from "../../components/Layout";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Search, ChevronLeft, ChevronRight } from "lucide-react";
import { motion } from "framer-motion";

const SearchPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.get("q") || "";
  const [searchInput, setSearchInput] = useState(query);
  const [currentPage, setCurrentPage] = useState(1);

  const { data: results, isLoading } = useSearchTvShows(query, currentPage);

  useEffect(() => {
    setSearchInput(query);
    setCurrentPage(1);
  }, [query]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchInput.trim()) {
      setSearchParams({ q: searchInput.trim() });
      setCurrentPage(1);
    }
  };

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
          <h1 className="text-4xl font-bold text-foreground">
            Search TV Shows
          </h1>

          <form onSubmit={handleSearch} className="flex gap-2">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-muted-foreground" />
              <Input
                type="search"
                placeholder="Search for TV shows..."
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                className="pl-10 h-12 text-lg text-foreground"
              />
            </div>
            <Button type="submit" size="lg" className="px-8">
              Search
            </Button>
          </form>

          {query && results && (
            <div className="text-muted-foreground">
              Found{" "}
              <span className="font-semibold text-foreground">
                {results.totalCount}
              </span>{" "}
              results for{" "}
              <span className="font-semibold text-foreground">"{query}"</span>
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

        {!isLoading && results && results.items.length > 0 && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: 0.1 }}
              className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6"
            >
              {results.items.map((show, index) => (
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

            {results.totalPages > 1 && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex items-center justify-center gap-4"
              >
                <Button
                  variant="outline"
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={!results.hasPreviousPage}
                >
                  <ChevronLeft className="mr-2 h-4 w-4" />
                  Previous
                </Button>

                <div className="flex items-center gap-2">
                  {[...Array(Math.min(5, results.totalPages))].map((_, i) => {
                    let pageNum;
                    if (results.totalPages <= 5) {
                      pageNum = i + 1;
                    } else if (currentPage <= 3) {
                      pageNum = i + 1;
                    } else if (currentPage >= results.totalPages - 2) {
                      pageNum = results.totalPages - 4 + i;
                    } else {
                      pageNum = currentPage - 2 + i;
                    }

                    if (pageNum < 1 || pageNum > results.totalPages)
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
                  disabled={!results.hasNextPage}
                >
                  Next
                  <ChevronRight className="ml-2 h-4 w-4" />
                </Button>
              </motion.div>
            )}
          </>
        )}

        {!isLoading && query && results && results.items.length === 0 && (
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center justify-center py-20 space-y-4"
          >
            <div className="text-6xl">🔍</div>
            <h3 className="text-2xl font-semibold">No results found</h3>
            <p className="text-muted-foreground">
              Try different keywords or check your spelling
            </p>
          </motion.div>
        )}

        {!query && (
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center justify-center py-20 space-y-4"
          >
            <Search className="h-24 w-24 text-muted-foreground" />
            <h3 className="text-2xl font-semibold">Start searching</h3>
            <p className="text-muted-foreground">
              Enter a TV show name to find what you&apos;re looking for
            </p>
          </motion.div>
        )}
      </div>
    </Layout>
  );
};

export default SearchPage;
