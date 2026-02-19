export interface VegProductCreation{
    id?: number;
    name: string;
    price: number;
    description?: string;
    stockQuantity?: number;
    netWeight?: number;
    idCategory?: number | null;
}