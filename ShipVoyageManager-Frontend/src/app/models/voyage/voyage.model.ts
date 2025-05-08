import { Guid } from "guid-typescript";

export interface IVoyage {
    id: Guid;
    startTime: Date;
    endTime: Date;
    shipId: Guid;
    shipName: string;
    departurePortId: Guid;
    departurePortName: string;
    arrivalPortId: Guid;
    arrivalPortName: string;
}