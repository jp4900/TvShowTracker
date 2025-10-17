import { useState } from "react";
import { Download, FileText, FileSpreadsheet, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import Layout from "@/components/Layout";
import { exportApi } from "@/services/api";
import { toast } from "sonner";

const Export = () => {
  const [loading, setLoading] = useState<string | null>(null);

  const handleExport = async (
    type: "mydata-csv" | "mydata-pdf" | "favorites-csv" | "favorites-pdf"
  ) => {
    setLoading(type);
    try {
      let blob: Blob;
      let filename: string;

      switch (type) {
        case "mydata-csv":
          blob = await exportApi.myDataCsv();
          filename = `my-data-${new Date().toISOString().split("T")[0]}.csv`;
          break;
        case "mydata-pdf":
          blob = await exportApi.myDataPdf();
          filename = `my-data-${new Date().toISOString().split("T")[0]}.pdf`;
          break;
        case "favorites-csv":
          blob = await exportApi.favoritesCsv();
          filename = `favorites-${new Date().toISOString().split("T")[0]}.csv`;
          break;
        case "favorites-pdf":
          blob = await exportApi.favoritesPdf();
          filename = `favorites-${new Date().toISOString().split("T")[0]}.pdf`;
          break;
      }

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);

      toast.success("Export successful!", {
        description: `Your ${
          type.includes("mydata") ? "data" : "favorites"
        } has been downloaded.`,
      });
    } catch (error) {
      console.error("Export error:", error);
      toast.error("Export failed", {
        description:
          "There was an error exporting your data. Please try again.",
      });
    } finally {
      setLoading(null);
    }
  };

  return (
    <Layout>
      <div className="container mx-auto px-4 py-8 max-w-4xl">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2 text-foreground">
            Export Your{" "}
            <span className="bg-gradient-to-r from-primary to-cyan-400 bg-clip-text text-transparent">
              Data
            </span>
          </h1>
          <p className="text-muted-foreground">
            Download your personal information and favorites in CSV or PDF
            format. In compliance with GDPR, you have the right to access and
            export your data.
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          <Card className="border-primary/20 hover:border-primary/40 transition-colors">
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-card-foreground">
                <Download className="h-5 w-5 text-primary" />
                My Personal Data
              </CardTitle>
              <CardDescription>
                Export all your personal information including account details,
                activity history, and preferences.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <Button
                onClick={() => handleExport("mydata-csv")}
                disabled={loading !== null}
                variant="outline"
                className="w-full justify-start"
              >
                {loading === "mydata-csv" ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <FileSpreadsheet className="mr-2 h-4 w-4" />
                )}
                Export as CSV
              </Button>
              <Button
                onClick={() => handleExport("mydata-pdf")}
                disabled={loading !== null}
                variant="outline"
                className="w-full justify-start"
              >
                {loading === "mydata-pdf" ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <FileText className="mr-2 h-4 w-4" />
                )}
                Export as PDF
              </Button>
            </CardContent>
          </Card>

          <Card className="border-primary/20 hover:border-primary/40 transition-colors">
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-card-foreground">
                <Download className="h-5 w-5 text-primary" />
                My Favorites
              </CardTitle>
              <CardDescription>
                Export your complete list of favorite TV shows with details and
                ratings.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <Button
                onClick={() => handleExport("favorites-csv")}
                disabled={loading !== null}
                variant="outline"
                className="w-full justify-start"
              >
                {loading === "favorites-csv" ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <FileSpreadsheet className="mr-2 h-4 w-4" />
                )}
                Export as CSV
              </Button>
              <Button
                onClick={() => handleExport("favorites-pdf")}
                disabled={loading !== null}
                variant="outline"
                className="w-full justify-start"
              >
                {loading === "favorites-pdf" ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <FileText className="mr-2 h-4 w-4" />
                )}
                Export as PDF
              </Button>
            </CardContent>
          </Card>
        </div>

        <Card className="mt-6 border-muted">
          <CardHeader>
            <CardTitle className="text-lg text-card-foreground">
              Your Data Rights
            </CardTitle>
          </CardHeader>
          <CardContent className="text-sm text-muted-foreground space-y-2">
            <p>
              <strong className="text-card-foreground">Right to Access:</strong>{" "}
              You can download all your personal data at any time.
            </p>
            <p>
              <strong className="text-card-foreground">
                Right to Portability:
              </strong>{" "}
              Your exported data is in standard formats (CSV, PDF) that can be
              used with other services.
            </p>
            <p>
              <strong className="text-card-foreground">
                Right to Erasure:
              </strong>{" "}
              If you wish to delete your account and all associated data, please
              contact support.
            </p>
            <p className="pt-2 text-xs">
              All exports are generated in real-time and contain your most
              current information. For questions about data privacy, please
              review our Privacy Policy or contact our Data Protection Officer.
            </p>
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
};

export default Export;
