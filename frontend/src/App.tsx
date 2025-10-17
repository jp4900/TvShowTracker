import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AuthProvider } from "./contexts/AuthContext";
import { Toaster } from "sonner";
import ProtectedRoute from "./components/ProtedtedRoute";

// Public pages
import LoginPage from "./pages/public/Login";
import RegisterPage from "./pages/public/Register";

// Private pages
import HomePage from "./pages/private/Home";
import TvShowPage from "./pages/private/TvShow";
import FavoritesPage from "./pages/private/Favorites";
import RecommendationsPage from "./pages/private/Recommendations";
import SearchPage from "./pages/private/Search";
import ExportPage from "./pages/private/Export";
import ActorsPage from "./pages/private/Actors";
import ActorPage from "./pages/private/Actor";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <HomePage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/show/:id"
              element={
                <ProtectedRoute>
                  <TvShowPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/favorites"
              element={
                <ProtectedRoute>
                  <FavoritesPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/recommendations"
              element={
                <ProtectedRoute>
                  <RecommendationsPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/search"
              element={
                <ProtectedRoute>
                  <SearchPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/actors"
              element={
                <ProtectedRoute>
                  <ActorsPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/actors/:id"
              element={
                <ProtectedRoute>
                  <ActorPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/export"
              element={
                <ProtectedRoute>
                  <ExportPage />
                </ProtectedRoute>
              }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </AuthProvider>
      </BrowserRouter>
      <Toaster richColors position="top-right" />
    </QueryClientProvider>
  );
}

export default App;
