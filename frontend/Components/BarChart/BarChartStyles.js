import { useColors } from '@Hooks/UseColors';
import { StyleSheet } from 'react-native';

const BarChartStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
        chartConfig : {
            backgroundGradientFrom: COLORS.main,
            backgroundGradientFromOpacity: 1,
            backgroundGradientTo: COLORS.main,
            backgroundGradientToOpacity: 1,
            color: () => COLORS.main_opposite,
            barPercentage: 0.5,
            fillShadowGradient: COLORS.light_primary,
            fillShadowGradientOpacity: 0.7,
        },
        chartStyle: {
            alignSelf: 'center',
        },
        predictionText: {
            color: COLORS.secondary,
        },
        scrollViewContainer: { 
            flexDirection: 'column'
        }
    });
};

export default BarChartStyles;