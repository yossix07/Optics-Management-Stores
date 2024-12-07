import { useContext } from "react";
import { ThemeContext } from "@Contexts/ThemeContext";

export const useColors = () => {
  const { colorPlatter } = useContext(ThemeContext);

  return colorPlatter;
};
