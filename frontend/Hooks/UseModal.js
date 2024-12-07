import { useContext } from "react";
import { ModalContext } from "@Contexts/ModalContext";

export const useModal = () => {
    const { showModal, hideModal } = useContext(ModalContext);
    return { showModal, hideModal };
};