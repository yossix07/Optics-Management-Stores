import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const HoledCircleStyles = (degress) => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    circleColor: COLORS.main_opposite,
    circleWrapper: {
        transform: [{ rotate: `${degress}deg` }],
    },
});
};

export default HoledCircleStyles;