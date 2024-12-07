import { useContext } from "react";
import { LoaderContext } from "@Contexts/LoaderContext";

export const useLoader = () => {
    const { isLoading, showLoader, hideLoader } = useContext(LoaderContext);
    return { isLoading, showLoader, hideLoader };
};