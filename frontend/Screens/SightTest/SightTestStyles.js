import { StyleSheet } from 'react-native';
import { BIG_FONT_SIZE, BASE_PADDING } from '@Utilities/Styles';

const SightTestStyles = () => {
  return StyleSheet.create({
    center: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    buttonsWrapper: {
        width: '100%',
        flexDirection: 'row',
        justifyContent: 'space-evenly',
        alignItems: 'center',
        padding: 2 * BASE_PADDING,
    },
    infoSection: {
        width: '100%',
        justifyContent: 'center',
        alignItems: 'center',
        marginTop: '10%',
    },
    mainCircleWrapper: {
        width: '100%',
        height: '50%',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: '30%',
    },
    testSection: {
        width: '100%',
        marginTop: '25%',
        height: '50%',
        justifyContent: 'space-between',
        alignItems: 'center',
    },
    instructions: {
        fontSize: BIG_FONT_SIZE,
        fontWeight: 'bold',
        textAlign: 'center',
        marginBottom: '8%',
    },
    openingScreen: {
        width: '100%',
        height: '90%',
        justifyContent: 'center',
        alignItems: 'center',
    },
    startButton: {
        width: '50%',
        position: 'absolute',
        bottom: 0,
    },
    resultText: {
        marginVertical: '7%',
    },
    scoreBarWrapper: {
        flexDirection: 'row',
    },
    scoreBar: {
        width: '75%',
    },
    });
};

export default SightTestStyles;