import { Link } from "react-router-dom";
import { User, Tv } from "lucide-react";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { ActorListItem } from "../types";
import { motion } from "framer-motion";

interface ActorCardProps {
  actor: ActorListItem;
}

const ActorCard = ({ actor }: ActorCardProps) => {
  const age = actor.dateOfBirth
    ? new Date().getFullYear() - new Date(actor.dateOfBirth).getFullYear()
    : null;

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      whileHover={{ y: -8 }}
    >
      <Link to={`/actors/${actor.id}`}>
        <Card className="group overflow-hidden border-muted hover:border-primary/50 transition-all duration-300 hover:shadow-2xl hover:shadow-primary/10">
          <div className="relative aspect-[2/3] overflow-hidden bg-muted">
            {actor.profilePath ? (
              <img
                src={actor.profilePath}
                alt={actor.name}
                className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-110"
                loading="lazy"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center text-6xl">
                <User className="h-24 w-24 text-muted-foreground" />
              </div>
            )}

            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

            {actor.tvShowCount > 0 && (
              <div className="absolute top-2 right-2 flex items-center space-x-1 rounded-full bg-background/80 backdrop-blur-sm px-2 py-1">
                <Tv className="h-3 w-3" />
                <span className="text-xs font-semibold">
                  {actor.tvShowCount}
                </span>
              </div>
            )}

            <div className="absolute bottom-0 left-0 right-0 p-4 transform translate-y-full group-hover:translate-y-0 transition-transform duration-300">
              {actor.placeOfBirth && (
                <Badge variant="secondary" className="text-xs">
                  {actor.placeOfBirth}
                </Badge>
              )}
            </div>
          </div>

          <div className="p-4 space-y-2">
            <h3 className="font-semibold line-clamp-2 group-hover:text-primary transition-colors">
              {actor.name}
            </h3>

            {age && (
              <p className="text-sm text-muted-foreground">{age} years old</p>
            )}
          </div>
        </Card>
      </Link>
    </motion.div>
  );
};

export default ActorCard;
