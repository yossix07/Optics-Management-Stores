import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const CounterStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    flexDirection: 'row',
    addIcon : {
        color: COLORS.primary
    },
    counterValue: {
        color: COLORS.main_opposite
    },
    subtractIcon : {
        color: COLORS.primary
    }
    });
};

export default CounterStyles;