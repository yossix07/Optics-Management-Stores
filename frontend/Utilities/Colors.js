const HEX_BASE = 16;
const BRIGHTNESS_THRESHOLD = 128;
const LIGHTER_COLOR_OFFSET = 55;
const DARKER_COLOR_OFFSET = 55;
const RED_LUMINANCE = 0.2126;
const GREEN_LUMINANCE = 0.7152;
const BLUE_LUMINANCE = 0.0722;
const RGB_MAX = 255;
const RGB_MIN = 0;
const PAD_SIZE = 6;
const WHITE = '#ffffff';
const BLACK = '#000000';
const LIGHTER_BLACK = '#28282B';
const GREY = '#c9c9c9';
const LIGHT_GREY = '#808080';
const BLUE = '#318CE7';
const LIGHT_BLUE = '#68C3FF';
const DARK_BLUE = '#0055B0';
const ORANGE = '#EB8258';

export const calculateColorBrightness = (hexValue) => {
    hexValue = hexValue.replace('#', '');
    
    // Convert hex to RGB
    const r = parseInt(hexValue.substring(0, 2), HEX_BASE);
    const g = parseInt(hexValue.substring(2, 4), HEX_BASE);
    const b = parseInt(hexValue.substring(4, 6), HEX_BASE);
    
    // Calculate relative luminance using the formula
    const luminance = RED_LUMINANCE * r + GREEN_LUMINANCE * g + BLUE_LUMINANCE * b;
    
    return luminance;
};

export const getLighterAndDarkerColors = (hexValue) => {
    hexValue = hexValue.replace('#', '');
  
    // Convert hex to RGB
    const r = parseInt(hexValue.substring(0, 2), HEX_BASE);
    const g = parseInt(hexValue.substring(2, 4), HEX_BASE);
    const b = parseInt(hexValue.substring(4, 6), HEX_BASE);
  
    // Calculate lighter color
    const lighterR = Math.min(r + LIGHTER_COLOR_OFFSET, RGB_MAX);
    const lighterG = Math.min(g + LIGHTER_COLOR_OFFSET, RGB_MAX);
    const lighterB = Math.min(b + LIGHTER_COLOR_OFFSET, RGB_MAX);
    const lighterHex = `#${(lighterR << 16 | lighterG << 8 | lighterB).toString(HEX_BASE).padStart(PAD_SIZE, '0')}`;
  
    // Calculate darker color
    const darkerR = Math.max(r - DARKER_COLOR_OFFSET, RGB_MIN);
    const darkerG = Math.max(g - DARKER_COLOR_OFFSET, RGB_MIN);
    const darkerB = Math.max(b - DARKER_COLOR_OFFSET, RGB_MIN);
    const darkerHex = `#${(darkerR << 16 | darkerG << 8 | darkerB).toString(HEX_BASE).padStart(PAD_SIZE, '0')}`;
  
    return {
      lighterHex,
      darkerHex
    };
};

export const generateCustomColorPlatter = (main, primary, secondary) => {
    const mainBrightness = calculateColorBrightness(main);
    const isMainDark = mainBrightness < BRIGHTNESS_THRESHOLD;

    const primaryBrightness = calculateColorBrightness(primary);
    const isPrimaryDark = primaryBrightness < BRIGHTNESS_THRESHOLD;

    const { lighterHex, darkerHex } = getLighterAndDarkerColors(primary);

    return {
        main: main,
        main_opposite: isMainDark ? WHITE : BLACK,
        grey: isMainDark ? LIGHT_GREY : GREY,

        primary: primary,
        primary_opposite: isPrimaryDark ? WHITE : BLACK,
        light_primary: lighterHex,
        dark_primary: darkerHex,
        secondary: secondary,
    };
};

export const darkColorPlatter = {
    main_opposite: WHITE,
    grey: LIGHT_GREY,
    main: LIGHTER_BLACK,

    primary: BLUE,
    primary_opposite: WHITE,
    light_primary: LIGHT_BLUE,
    dark_primary: DARK_BLUE,
    secondary: ORANGE,
};

export const lightColorPlatter = {
    main: WHITE,
    grey: GREY,
    main_opposite: LIGHTER_BLACK,

    primary: BLUE,
    primary_opposite: LIGHTER_BLACK,
    light_primary: LIGHT_BLUE,
    dark_primary: DARK_BLUE,
    secondary: ORANGE
};