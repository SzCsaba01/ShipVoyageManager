import { Guid } from 'guid-typescript';
import { IVoyage } from '../voyage/voyage.model';

export interface IPort {
    id: Guid;
    name: string;
    countryName: string;
    departingVoyages: IVoyage[];
    arrivingVoyages: IVoyage[];
}