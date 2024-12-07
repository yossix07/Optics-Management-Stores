import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN, BASE_PADDING } from '@Utilities/Styles'; 

const boxInfoStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    boxInfo: {
        marginVertical: BASE_MARGIN,
        marginLeft: 2 * BASE_MARGIN,
        paddingTop: BASE_PADDING,
        paddingLeft: 2 * BASE_PADDING,
    },
    boxInfoField : {
        flexDirection: 'row',
        alignItems: 'center',
    },
    boxInfoFunctionalField : {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
    },
    editIcons: {
        flexDirection: 'row',
    },
    boxInfoIcon: {
        color: COLORS.secondary,
        marginRight: BASE_MARGIN,
    },
    textWrapper: {
        flexDirection: 'row',
        alignItems: 'center',
    },
    text: {
      color: COLORS.main_opposite,
    },
    iconButton: {
        color: COLORS.primary,
    },
    });
};

export default boxInfoStyles;