import { StyleSheet, Dimensions } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { SMALL_FONT_SIZE, MEDIUM_FONT_SIZE, BASE_MARGIN, BASE_PADDING } from '@Utilities/Styles';

const ClickableItemStyles = () => {
  const COLORS = useColors();

  const { width } = Dimensions.get('window');
  const imageWidth = width / 3.5;
  const itemHeight = imageWidth + 20;

  return StyleSheet.create({
    container: {
        height: itemHeight,
        flexDirection: 'row',
        justifyContent: 'space-between',
        padding: 2 * BASE_PADDING,
        backgroundColor: COLORS.main,
    },
    image: {
        width: imageWidth,
        height: imageWidth,
        borderColor: COLORS.main_opposite,
        borderWidth: 1,
        borderRadius: 5,
    },
    itemInfo: {
        flex: 1,
        marginLeft: 2 * BASE_MARGIN,
    },
    namePriceWrapper: {
        flexDirection: 'row',
        alignItems: 'center',
        marginBottom: BASE_MARGIN,
    },
    name: {
        flex: 1,
        marginRight: 2 * BASE_MARGIN,
        fontSize: MEDIUM_FONT_SIZE,
        fontWeight: 'bold',
    },
    price: {
        fontSize: MEDIUM_FONT_SIZE,
    },
    descriptionWrapper: {
        flex: 1,
    },
    description: {
        fontSize: SMALL_FONT_SIZE,
    },
    icon: {
        position:'absolute',
        top: '45%',
        right:'2%',
    }
    });
};

export default ClickableItemStyles;