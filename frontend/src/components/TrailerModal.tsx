import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Loader2 } from "lucide-react";
import { tmdbApi } from "../services/tmdb";

interface TrailerModalProps {
  showTitle: string;
  isOpen: boolean;
  onClose: () => void;
}

export function TrailerModal({
  showTitle,
  isOpen,
  onClose,
}: TrailerModalProps) {
  const [trailerKey, setTrailerKey] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);

  useEffect(() => {
    const fetchTrailer = async () => {
      if (!isOpen) return;

      setLoading(true);
      setError(false);

      try {
        const tmdbId = await tmdbApi.searchTvShow(showTitle);

        if (tmdbId) {
          const key = await tmdbApi.getTrailer(tmdbId);
          setTrailerKey(key);

          if (!key) {
            setError(true);
          }
        } else {
          setError(true);
        }
      } catch (err) {
        console.error("Error loading trailer:", err);
        setError(true);
      } finally {
        setLoading(false);
      }
    };

    fetchTrailer();
  }, [isOpen, showTitle]);

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl">
        <DialogHeader>
          <DialogTitle>{showTitle} - Trailer</DialogTitle>
        </DialogHeader>

        <div className="aspect-video bg-muted rounded-lg overflow-hidden">
          {loading && (
            <div className="flex items-center justify-center h-full">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
          )}

          {error && !loading && (
            <div className="flex flex-col items-center justify-center h-full space-y-4">
              <p className="text-muted-foreground">Trailer not available</p>
              <Button variant="outline" onClick={onClose}>
                Close
              </Button>
            </div>
          )}

          {trailerKey && !loading && !error && (
            <iframe
              src={tmdbApi.getYouTubeEmbedUrl(trailerKey)}
              title={`${showTitle} Trailer`}
              className="w-full h-full"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowFullScreen
            />
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
