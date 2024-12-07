import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN } from '@Utilities/Styles';

const StatsScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    header: {
        marginTop: '3%',
        justifyContent: 'center',
        alignItems: 'center',
    },
    dateField: {
        flexDirection: 'row',
        width: '80%',
        marginBottom: '5%',
        justifyContent: 'space-between',
        alignItems: 'center',
    },
    generalInfo: {
        marginTop: 1.5 * BASE_MARGIN
    },
    clickableColor: COLORS.primary,
    });
};

export default StatsScreenStyles;