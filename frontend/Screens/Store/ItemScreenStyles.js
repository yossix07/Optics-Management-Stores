import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { Dimensions } from 'react-native';
import { BIG_FONT_SIZE, EXTRA_LARGE_FONT_SIZE } from '@Utilities/Styles';

const IMAGE_HEIGHT = 0.35 * Dimensions.get('window').height;

const ItemScreenStyles = () => {
  const COLORS = useColors();
  const { width } = Dimensions.get('window');
  
  return StyleSheet.create({
    container: {
        backgroundColor: COLORS.main,
        height: '100%',
    },
    image: {
        width: width,
        height: IMAGE_HEIGHT,
        borderColor: COLORS.main_opposite,
        borderWidth: 2,
    },
    info: {
        marginLeft: "1%",
        marginRight: '1%',
    },
    namePriceWrapper: {
        flexDirection: 'row',
        alignItems: 'center',
        alignContent: 'space-around',
    },
    name: {
        fontSize: EXTRA_LARGE_FONT_SIZE,
        fontWeight: 'bold',
        color: COLORS.main_opposite,
    },
    price: {
        marginLeft: 'auto',
        color: COLORS.main_opposite
    },
    description: {
        color: COLORS.main_opposite
    },
    addToCart: {
        position: 'absolute',
        width: '95%',
        bottom: 0,
    },
    buttons: {
        width: '100%',
        flexDirection: 'row',
        justifyContent: 'space-between',
    },
    trashButton: {
        width: '45%',
    },
    chartWrapper: {
        marginVertical: '8%',
    },
    chartTitle: {
        fontSize: BIG_FONT_SIZE,
        fontWeight: 'bold',
        color: COLORS.main_opposite,
    },
});
};

export default ItemScreenStyles;