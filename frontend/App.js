import 'react-native-gesture-handler';
import React from "react";
import Navigation from "./Navigation";
import { UserProvider } from "./Contexts/UserContext";
import { ThemeProvider } from "./Contexts/ThemeContext";
import Toast from 'react-native-toast-message';
import { LoaderProvider } from "./Contexts/LoaderContext";
import Loader from "./Components/Loader/Loader";
import { useLoader } from "./Hooks/UseLoader";
import { ModalProvider } from './Contexts/ModalContext';
import { LogBox } from "react-native";

LogBox.ignoreAllLogs(true);

export default function App() {
  return (
    <>
      <LoaderProvider>
        <UserProvider>
          <ThemeProvider>
            <ModalProvider>
              <Navigation/>
              <InnerLoader/>
            </ModalProvider>
          </ThemeProvider>
        </UserProvider>
      </LoaderProvider>
      <Toast />
    </>
  );
}

const InnerLoader = () => {
  const { isLoading } = useLoader();
  return isLoading ? <Loader/> : null;
};