import { useState } from "react";
import { useActors } from "../../hooks/useActors";
import ActorCard from "../../components/ActorCard";
import Layout from "../../components/Layout";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  ChevronLeft,
  ChevronRight,
  Search,
  Users,
  ArrowUpIcon,
  Filter,
} from "lucide-react";
import { motion } from "framer-motion";

const ActorsPage = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const [sortBy, setSortBy] = useState("popularity");
  const [sortDesc, setSortDesc] = useState(true);
  const [search, setSearch] = useState("");
  const [searchInput, setSearchInput] = useState("");

  const { data: actors, isLoading } = useActors({
    pageNumber: currentPage,
    pageSize: 20,
    search: search || undefined,
    sortBy,
    sortDesc,
  });

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setSearch(searchInput);
    setCurrentPage(1);
  };

  const toggleSortOrder = () => {
    setSortDesc(!sortDesc);
    setCurrentPage(1);
  };

  return (
    <Layout>
      <div className="space-y-8">
        {/* Hero Section */}
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-primary/20 via-primary/10 to-background border border-primary/20 p-8 md:p-12"
        >
          <div className="absolute inset-0 bg-grid-white/[0.02] -z-10" />
          <div className="relative z-10 space-y-4">
            <div className="inline-flex items-center space-x-2 rounded-full bg-primary/20 px-4 py-1.5 text-sm font-medium text-primary">
              <Users className="h-4 w-4" />
              <span>Discover Talented Actors</span>
            </div>
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold tracking-tight text-foreground">
              Explore Amazing
              <br />
              <span className="bg-gradient-to-r from-primary to-cyan-400 bg-clip-text text-transparent">
                Actors
              </span>
            </h1>
            <p className="text-muted-foreground text-lg max-w-2xl">
              Browse through talented actors and discover their amazing
              performances
            </p>
          </div>
        </motion.div>

        {/* Search & Filters */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
          className="space-y-4"
        >
          {/* Search Bar */}
          <form onSubmit={handleSearch} className="flex gap-2">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Search actors by name..."
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                className="pl-10"
              />
            </div>
            <Button type="submit">Search</Button>
            {search && (
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  setSearch("");
                  setSearchInput("");
                  setCurrentPage(1);
                }}
              >
                Clear
              </Button>
            )}
          </form>

          {/* Filters */}
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 p-4 rounded-xl border bg-card">
            <div className="flex items-center space-x-2">
              <Filter className="h-5 w-5 text-muted-foreground" />
              <span className="font-medium text-card-foreground">Filters</span>
            </div>

            <div className="flex flex-wrap gap-3 w-full sm:w-auto">
              <Select
                value={sortBy}
                onValueChange={(value) => {
                  setSortBy(value);
                  setCurrentPage(1);
                }}
              >
                <SelectTrigger className="w-full sm:w-[180px] text-foreground">
                  <SelectValue placeholder="Sort by" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="popularity">Most Popular</SelectItem>
                  <SelectItem value="name">Name (A-Z)</SelectItem>
                  <SelectItem value="dateofbirth">Age</SelectItem>
                </SelectContent>
              </Select>

              <Button
                variant="outline"
                size="icon"
                onClick={toggleSortOrder}
                className="h-10 w-10 text-muted-foreground"
                title={sortDesc ? "Descending" : "Ascending"}
              >
                <ArrowUpIcon
                  className={`h-4 w-4 transition-transform ${
                    sortDesc ? "rotate-0" : "rotate-180"
                  }`}
                />
              </Button>

              {actors && (
                <div className="hidden lg:flex items-center px-4 py-2 rounded-lg bg-muted text-sm text-muted-foreground">
                  <span className="font-medium text-foreground">
                    {actors.totalCount}
                  </span>
                  <span className="ml-1">actors found</span>
                </div>
              )}
            </div>
          </div>
        </motion.div>

        {/* Loading State */}
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

        {/* Actors Grid */}
        {!isLoading && actors && actors.items.length > 0 && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: 0.2 }}
              className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6"
            >
              {actors.items.map((actor, index) => (
                <motion.div
                  key={actor.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.05 }}
                >
                  <ActorCard actor={actor} />
                </motion.div>
              ))}
            </motion.div>

            {/* Pagination */}
            {actors.totalPages > 1 && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex flex-col sm:flex-row items-center justify-center gap-4 pt-8"
              >
                <Button
                  variant="outline"
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={!actors.hasPreviousPage}
                  className="w-full sm:w-auto"
                >
                  <ChevronLeft className="mr-2 h-4 w-4" />
                  Previous
                </Button>

                <div className="flex items-center gap-2">
                  {[...Array(Math.min(5, actors.totalPages))].map((_, i) => {
                    let pageNum;
                    if (actors.totalPages <= 5) {
                      pageNum = i + 1;
                    } else if (currentPage <= 3) {
                      pageNum = i + 1;
                    } else if (currentPage >= actors.totalPages - 2) {
                      pageNum = actors.totalPages - 4 + i;
                    } else {
                      pageNum = currentPage - 2 + i;
                    }

                    if (pageNum < 1 || pageNum > actors.totalPages) return null;

                    return (
                      <Button
                        key={pageNum}
                        variant={
                          currentPage === pageNum ? "default" : "outline"
                        }
                        size="icon"
                        onClick={() => handlePageChange(pageNum)}
                        className={
                          currentPage === pageNum
                            ? "shadow-lg shadow-primary/50"
                            : ""
                        }
                      >
                        {pageNum}
                      </Button>
                    );
                  })}
                </div>

                <Button
                  variant="outline"
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={!actors.hasNextPage}
                  className="w-full sm:w-auto"
                >
                  Next
                  <ChevronRight className="ml-2 h-4 w-4" />
                </Button>
              </motion.div>
            )}
          </>
        )}

        {/* Empty State */}
        {!isLoading && actors && actors.items.length === 0 && (
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center justify-center py-20 space-y-4"
          >
            <Users className="h-16 w-16 text-muted-foreground" />
            <h3 className="text-2xl font-semibold">No actors found</h3>
            <p className="text-muted-foreground">
              {search
                ? `No results for "${search}"`
                : "Try adjusting your filters"}
            </p>
            {search && (
              <Button
                variant="outline"
                onClick={() => {
                  setSearch("");
                  setSearchInput("");
                  setCurrentPage(1);
                }}
              >
                Clear Search
              </Button>
            )}
          </motion.div>
        )}
      </div>
    </Layout>
  );
};

export default ActorsPage;
