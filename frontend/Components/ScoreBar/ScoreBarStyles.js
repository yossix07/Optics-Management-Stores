import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const HEIGHT = 20;

const ScoreBarStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    container: {
        height: HEIGHT,
        width: '100%',
        backgroundColor: COLORS.main_opposite,
        borderRadius: 10,
        overflow: 'hidden',
      },
      barContainer: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'center',
      },
      fill: {
        height: '100%',
        backgroundColor: COLORS.dark_primary,
      },
  });
};

export default ScoreBarStyles;