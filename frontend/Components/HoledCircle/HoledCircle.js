import React from "react";
import { View } from "react-native";
import { Svg, Path } from "react-native-svg";
import HoledCircleStyles from "./HoledCircleStyles";
import { CIRCLE_D, MIN_X, MIN_Y, WIDTH, HEIGHT } from "./HoledCircleSvgConfig";

const HoledCircle = ({ height, width, degree = 0, style }) => {

  const styles = HoledCircleStyles(degree);

  return (
    <View style={ styles.circleWrapper }>
      <Svg style={ style } height={ height } width={ width } viewBox={ `${MIN_X} ${MIN_Y} ${WIDTH} ${HEIGHT}` } >
        <Path
          fill={ styles.circleColor }
          d={ CIRCLE_D }
        />
      </Svg>
    </View>
  );
};

export default HoledCircle;
