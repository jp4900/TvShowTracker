import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import {
  Home,
  Heart,
  Sparkles,
  LogOut,
  Menu,
  Search,
  Download,
  User,
  Users,
} from "lucide-react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { ThemeToggle } from "./ThemeToggle";

const Navbar = () => {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchQuery)}`);
      setSearchQuery("");
      setMobileMenuOpen(false);
    }
  };

  return (
    <nav className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto px-4">
        <div className="flex h-16 items-center justify-between">
          <Link to="/" className="flex items-center space-x-2 group">
            <span className="text-2xl">🎬</span>
            <span className="hidden md:block text-xl font-bold bg-gradient-to-r from-primary to-cyan-400 bg-clip-text text-transparent">
              TV Show Tracker
            </span>
            <span className="md:hidden text-xl font-bold bg-gradient-to-r from-primary to-cyan-400 bg-clip-text text-transparent">
              TVT
            </span>
          </Link>

          {isAuthenticated && (
            <form
              onSubmit={handleSearch}
              className="hidden md:block flex-1 max-w-md mx-8"
            >
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  type="search"
                  placeholder="Search TV shows..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pl-10 text-foreground"
                />
              </div>
            </form>
          )}

          {isAuthenticated ? (
            <div className="hidden md:flex items-center space-x-2 text-muted-foreground">
              <Button variant="ghost" asChild>
                <Link to="/" className="flex items-center space-x-2">
                  <Home className="h-4 w-4" />
                  <span>Home</span>
                </Link>
              </Button>

              <Button variant="ghost" asChild>
                <Link to="/actors" className="flex items-center space-x-2">
                  <Users className="h-4 w-4" />
                  <span>Actors</span>
                </Link>
              </Button>

              <Button variant="ghost" asChild>
                <Link to="/favorites" className="flex items-center space-x-2">
                  <Heart className="h-4 w-4" />
                  <span>Favorites</span>
                </Link>
              </Button>

              <Button variant="ghost" asChild>
                <Link
                  to="/recommendations"
                  className="flex items-center space-x-2"
                >
                  <Sparkles className="h-4 w-4" />
                  <span>For You</span>
                </Link>
              </Button>

              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    variant="ghost"
                    className="flex items-center space-x-2"
                  >
                    <User className="h-6 w-6 text-muted-foreground" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-56">
                  <DropdownMenuItem asChild></DropdownMenuItem>
                  <DropdownMenuItem asChild>
                    <Link to="/export" className="cursor-pointer">
                      <Download className="mr-2 h-4 w-4" />
                      Export Data
                    </Link>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    onClick={logout}
                    className="cursor-pointer text-destructive"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    Logout
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
              <ThemeToggle />
            </div>
          ) : (
            <div className="hidden md:flex items-center space-x-2">
              <Button variant="ghost" asChild>
                <Link to="/login">Login</Link>
              </Button>
              <Button asChild>
                <Link to="/register">Sign Up</Link>
              </Button>
            </div>
          )}

          <Button
            variant="ghost"
            size="icon"
            className="md:hidden text-muted-foreground"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          >
            <Menu className="h-5 w-5" />
          </Button>
        </div>

        {mobileMenuOpen && (
          <div className="md:hidden py-4 border-t">
            {isAuthenticated ? (
              <div className="space-y-4">
                <form onSubmit={handleSearch}>
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      type="search"
                      placeholder="Search TV shows..."
                      value={searchQuery}
                      onChange={(e) => setSearchQuery(e.target.value)}
                      className="pl-10 text-muted-foreground"
                    />
                  </div>
                </form>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                >
                  <Link
                    to="/"
                    onClick={() => setMobileMenuOpen(false)}
                    className="text-muted-foreground"
                  >
                    <Home className="mr-2 h-4 w-4 text-muted-foreground" />
                    Home
                  </Link>
                </Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                >
                  <Link
                    to="/actors"
                    onClick={() => setMobileMenuOpen(false)}
                    className="text-muted-foreground"
                  >
                    <Users className="mr-2 h-4 w-4 text-muted-foreground" />
                    Actors
                  </Link>
                </Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                >
                  <Link
                    to="/favorites"
                    onClick={() => setMobileMenuOpen(false)}
                    className="text-muted-foreground"
                  >
                    <Heart className="mr-2 h-4 w-4 text-muted-foreground" />
                    Favorites
                  </Link>
                </Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                >
                  <Link
                    to="/recommendations"
                    onClick={() => setMobileMenuOpen(false)}
                    className="text-muted-foreground"
                  >
                    <Sparkles className="mr-2 h-4 w-4 text-muted-foreground" />
                    For You
                  </Link>
                </Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                ></Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  asChild
                >
                  <Link
                    to="/export"
                    onClick={() => setMobileMenuOpen(false)}
                    className="text-muted-foreground"
                  >
                    <Download className="mr-2 h-4 w-4 text-muted-foreground" />
                    Export Data
                  </Link>
                </Button>

                <Button
                  variant="ghost"
                  className="w-full justify-start text-destructive"
                  onClick={() => {
                    logout();
                    setMobileMenuOpen(false);
                  }}
                >
                  <LogOut className="mr-2 h-4 w-4 text-muted-foreground" />
                  Logout
                </Button>
              </div>
            ) : (
              <div className="space-y-2">
                <Button variant="ghost" className="w-full" asChild>
                  <Link to="/login" onClick={() => setMobileMenuOpen(false)}>
                    Login
                  </Link>
                </Button>
                <Button className="w-full" asChild>
                  <Link to="/register" onClick={() => setMobileMenuOpen(false)}>
                    Sign Up
                  </Link>
                </Button>
              </div>
            )}
          </div>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
