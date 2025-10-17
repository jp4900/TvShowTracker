import {
  ThemeProvider as NextThemesProvider,
  type Attribute,
} from "next-themes";

interface ThemeProviderProps {
  children: React.ReactNode;
  attribute?: string;
  defaultTheme?: string;
  enableSystem?: boolean;
}

export function ThemeProvider({
  children,
  attribute,
  ...props
}: ThemeProviderProps) {
  return (
    <NextThemesProvider
      attribute={attribute ? (attribute as Attribute) : undefined}
      {...props}
    >
      {children}
    </NextThemesProvider>
  );
}
