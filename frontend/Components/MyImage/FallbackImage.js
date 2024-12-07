
import React from "react";
import { View } from "react-native-animatable";
import Svg, { Path } from 'react-native-svg';
import { useColors } from "@Hooks/UseColors";
import { MIN_X, MIN_Y, WIDTH, HEIGHT, NOT_FOUND_D} from "./FallbackImageSvgConfig";

const DEFUALT_SIZE = 100;

const FallbackImage = ({ style }) => {
    const COLORS = useColors();
    return (
        <View style={ [style, { justifyContent: "center", alignItems: "center" }] }>
            <Svg
                viewBox={ `${MIN_X} ${MIN_Y} ${WIDTH} ${HEIGHT}` }
                width={ style?.width * 0.9 || DEFUALT_SIZE }
                height={ style?.height * 0.9 || DEFUALT_SIZE }
                >
            <Path
                d={ NOT_FOUND_D }
                fill={ COLORS.main_opposite }
            />
            </Svg>
        </View>
    );
};

export default FallbackImage;