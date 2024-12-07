module.exports = function(api) {
  api.cache(true);
  return {
    presets: ['module:metro-react-native-babel-preset', 'babel-preset-expo'],
    plugins: [
      [
        'module-resolver',
        {
          root: ['.'],
          extensions: ['.js', '.json'],
          alias: {
            '@Services': './Services',
            '@assets': './assets',
            '@Components': './Components',
            '@Screens': './Screens',
            '@Utilities': './Utilities',
            '@Contexts': './Contexts',
            '@Hooks': './Hooks',
          },
        },
      ],
      'react-native-reanimated/plugin',
    ],
  };
};