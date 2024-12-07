import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { MEDIUM_FONT_SIZE, LARGE_FONT_SIZE, BASE_MARGIN } from '@Utilities/Styles';

const ItemFormScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
        container: {
            paddingTop: '10%'
        },
        title: {
            fontSize: LARGE_FONT_SIZE,
            fontWeight: "bold",
            marginBottom: '15%',
        },
        center: {
        alignItems: "center",
        justifyContent: "center",
        },
        imageButtons: {
            flexDirection: 'row',
            justifyContent: 'space-between',
        },
        buttonTitle: {
            color: COLORS.primary_opposite,
            fontSize: MEDIUM_FONT_SIZE
        },
        submitButtonText: {
            fontSize: MEDIUM_FONT_SIZE,
            fontWeight: 'bold',
        }
    });
};

export default ItemFormScreenStyles;