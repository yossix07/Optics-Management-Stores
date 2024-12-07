import React, { createContext, useState, useEffect, useMemo } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { lightColorPlatter, darkColorPlatter } from '@Utilities/Colors';
import { tenantID } from '../env';
import { generateCustomColorPlatter } from "@Utilities/Colors";
import { LIGHT, DARK, CUSTOM } from '@Utilities/Constants';
export const ThemeContext = createContext();

const themeItemName = `theme-${tenantID}`;
const colorItemName = `colors-${tenantID}`;
const NUM_OF_CUSTOM_COLORS = 3;

export function ThemeProvider({ children }) {
    const [ theme, setTheme ] = useState(LIGHT);
    const [ customColors, setCustomColors ] = useState({});
    const [ colorPlatter, setColorPlatter ] = useState({});

    useEffect(() => {
        fetchLocalTheme();
        fetchLocalColors();
    },[]);
    
    // save theme to local storage
    useEffect(() => {
        AsyncStorage.setItem(themeItemName, theme);
        if (Object.keys(customColors).length !== NUM_OF_CUSTOM_COLORS) {
            if (theme === LIGHT) {
                setCustomColors(lightColorPlatter);
            } else if (theme === DARK) {
                setCustomColors(darkColorPlatter);
            }
        }
    },[theme]);

    // in case of valid number of custom colors, save them to local storage
    useEffect(() => {
        if(Object.keys(customColors).length === NUM_OF_CUSTOM_COLORS) {
            AsyncStorage.setItem(colorItemName, JSON.stringify(customColors));
        }
    },[customColors]);

    // set color platter based on theme and custom colors
    useMemo(() => {
        if (theme === DARK) {
            setColorPlatter(darkColorPlatter);
        } else if (theme === LIGHT) {
            setColorPlatter(lightColorPlatter);
        } else if (theme === CUSTOM) {
            setColorPlatter(generateCustomColorPlatter(customColors?.main, customColors?.primary, customColors?.secondary));
        }
    }, [theme, customColors.main, customColors.primary, customColors.secondary]);
    
    const setCustomTheme = (main, primary, secondary) => {
        setCustomColors({
            main: main,
            primary: primary,
            secondary: secondary,
        });
    };
    
    // get theme from local storage
    const fetchLocalTheme = async () => {
        const localTheme = await AsyncStorage.getItem(themeItemName);
        if (localTheme){
            setTheme(localTheme);
        }
    };
    
    // get custom colors from local storage
    const fetchLocalColors = async () => {
        const localColors = await AsyncStorage.getItem(colorItemName);
        const parsedColors = JSON.parse(localColors);
        if (parsedColors && Object.keys(parsedColors).length != 0) {
            setCustomColors(parsedColors);
        }
    };

    return (
        <ThemeContext.Provider value={{ theme, setCustomTheme, customColors, setTheme, colorPlatter }}>
        { children }
        </ThemeContext.Provider>
    );
};