import React, { createContext, useState } from 'react';

export const LoaderContext = createContext();

export function LoaderProvider({ children }) {
    const [ isLoading, setIsloading ] = useState(false);

    const showLoader = () => {
        setIsloading(true);
    };
    
    const hideLoader = () => {
        setIsloading(false);
    };
    
    return (
        <LoaderContext.Provider value={{ isLoading, showLoader, hideLoader }}>
            { children }
        </LoaderContext.Provider>
    );
};