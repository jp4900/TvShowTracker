import { useRecommendations, useTrainModel } from "../../hooks/useTvShows";
import Layout from "../../components/Layout";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Sparkles,
  Brain,
  TrendingUp,
  Zap,
  RefreshCw,
  Heart,
} from "lucide-react";
import { motion } from "framer-motion";
import { Link } from "react-router-dom";
import { toast } from "sonner";

const RecommendationsPage = () => {
  const { data: recommendations, isLoading, refetch } = useRecommendations(12);
  const trainModel = useTrainModel();

  const handleTrainModel = async () => {
    try {
      await trainModel.mutateAsync();
      await refetch();
      toast.success("AI model trained!", {
        description: "Your recommendations have been updated",
      });
    } catch (error) {
      toast.error("Failed to train model");
    }
  };

  return (
    <Layout>
      <div className="space-y-8">
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-primary/20 via-purple-500/10 to-background border border-primary/20 p-8 md:p-12"
        >
          <div className="absolute inset-0 bg-grid-white/[0.02]" />
          <div className="relative z-10 space-y-4">
            <div className="inline-flex items-center space-x-2 rounded-full bg-primary/20 px-4 py-1.5 text-sm font-medium text-primary">
              <Brain className="h-4 w-4" />
              <span>AI-Powered Recommendations</span>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold tracking-tight text-foreground">
              Curated Just
              <br />
              <span className="bg-gradient-to-r from-primary via-purple-400 to-cyan-400 bg-clip-text text-transparent">
                For You
              </span>
            </h1>
            <p className="text-muted-foreground text-lg max-w-2xl">
              Powered by machine learning, these recommendations are based on
              your favorite shows and viewing patterns
            </p>

            <div className="flex flex-wrap gap-3 pt-4">
              <Button
                variant="secondary"
                onClick={handleTrainModel}
                disabled={trainModel.isPending}
              >
                {trainModel.isPending ? (
                  <>
                    <RefreshCw className="mr-2 h-4 w-4 animate-spin" />
                    Training...
                  </>
                ) : (
                  <>
                    <Zap className="mr-2 h-4 w-4" />
                    Retrain AI Model
                  </>
                )}
              </Button>
              <Button
                variant="outline"
                asChild
                className="text-muted-foreground"
              >
                <Link to="/favorites">
                  <Heart className="mr-2 h-4 w-4" />
                  Manage Favorites
                </Link>
              </Button>
            </div>
          </div>
        </motion.div>

        {isLoading && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(9)].map((_, i) => (
              <Card key={i} className="p-6 space-y-4">
                <Skeleton className="aspect-video w-full rounded-lg" />
                <Skeleton className="h-4 w-3/4" />
                <Skeleton className="h-3 w-1/2" />
              </Card>
            ))}
          </div>
        )}

        {!isLoading && recommendations && recommendations.length > 0 && (
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <h2 className="text-2xl font-semibold">Top Picks for You</h2>
              <Badge variant="secondary" className="text-sm">
                <TrendingUp className="mr-1 h-3 w-3" />
                {recommendations.length} recommendations
              </Badge>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {recommendations.map((rec, index) => (
                <motion.div
                  key={rec.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.05 }}
                >
                  <Link to={`/show/${rec.id}`}>
                    <Card className="group overflow-hidden hover:border-primary/50 transition-all duration-300 hover:shadow-2xl hover:shadow-primary/10">
                      <div className="relative aspect-video bg-muted overflow-hidden">
                        {rec.posterUrl ? (
                          <img
                            src={rec.posterUrl}
                            alt={rec.title}
                            className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110"
                          />
                        ) : (
                          <div className="flex items-center justify-center w-full h-full text-4xl">
                            🎬
                          </div>
                        )}

                        <div className="absolute top-3 right-3 bg-primary/90 backdrop-blur-sm text-primary-foreground px-3 py-1 rounded-full text-sm font-bold">
                          {rec.score.toFixed(0)}% Match
                        </div>

                        <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent opacity-60" />
                      </div>

                      <div className="p-6 space-y-3">
                        <h3 className="font-semibold text-lg group-hover:text-primary transition-colors line-clamp-2">
                          {rec.title}
                        </h3>

                        <div className="flex items-start space-x-2 text-sm">
                          <Sparkles className="h-4 w-4 text-primary flex-shrink-0 mt-0.5" />
                          <p className="text-muted-foreground line-clamp-2">
                            {rec.reason}
                          </p>
                        </div>

                        <div className="flex flex-wrap gap-2">
                          {rec.genres.slice(0, 3).map((genre) => (
                            <Badge
                              key={genre}
                              variant="secondary"
                              className="text-xs"
                            >
                              {genre}
                            </Badge>
                          ))}
                        </div>

                        <div className="flex items-center space-x-2 text-sm text-muted-foreground">
                          <div className="flex items-center space-x-1">
                            <span className="text-amber-400">⭐</span>
                            <span className="font-semibold text-foreground">
                              {rec.rating.toFixed(1)}
                            </span>
                          </div>
                          {rec.releaseDate && (
                            <>
                              <span>•</span>
                              <span>
                                {new Date(rec.releaseDate).getFullYear()}
                              </span>
                            </>
                          )}
                        </div>
                      </div>
                    </Card>
                  </Link>
                </motion.div>
              ))}
            </div>
          </div>
        )}

        {!isLoading && (!recommendations || recommendations.length === 0) && (
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center justify-center py-20 space-y-6"
          >
            <div className="relative">
              <div className="absolute inset-0 bg-primary/20 blur-3xl rounded-full" />
              <Brain className="relative h-24 w-24 text-muted-foreground" />
            </div>
            <div className="text-center space-y-2">
              <h3 className="text-2xl font-semibold">No recommendations yet</h3>
              <p className="text-muted-foreground max-w-md">
                Add some shows to your favorites to get personalized AI-powered
                recommendations
              </p>
            </div>
            <Button asChild size="lg">
              <Link to="/">
                <Sparkles className="mr-2 h-5 w-5" />
                Start Exploring
              </Link>
            </Button>
          </motion.div>
        )}
      </div>
    </Layout>
  );
};

export default RecommendationsPage;
