import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN, BASE_PADDING } from '@Utilities/Styles';

const SMALL_CARD_HEIGHT = 150;
const BIG_CARD_HEIGHT = 200;
const ELEVATION = 8;

const cardStyles = (small, fitContent) => {
  const COLORS = useColors();

  return StyleSheet.create({
    cardContainer: {
      margin: 2 * BASE_MARGIN,
      padding: 2 * BASE_PADDING,
      backgroundColor: COLORS.main,
      borderWidth: 1,
      borderRadius: 15,
      borderColor: COLORS.main,
      elevation: ELEVATION,
      shadowColor: COLORS.main_opposite,
      height: fitContent ? null : (small ? SMALL_CARD_HEIGHT : BIG_CARD_HEIGHT),
    },
    TitleRow: {
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginBottom: BASE_MARGIN,
    },
    IconTitleWrapper: {
      flexDirection: 'row',
      alignItems: 'center',
    },
    Title: {
      color: COLORS.main_opposite,
      fontWeight: "bold",
    },
    titleButton: {
      color: COLORS.primary,
    },
    icon: {
      color: COLORS.secondary,
    },
  });
};

export default cardStyles;