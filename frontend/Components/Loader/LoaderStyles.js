import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const zIndex = 100;

const LoaderStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    color: COLORS.secondary,
    loaderContainer: {
        position: 'absolute',
        height: '100%',
        width: '100%',
        zIndex: zIndex,
        justifyContent: 'center',
        alignItems: 'center',
    },
    });
};

export default LoaderStyles;