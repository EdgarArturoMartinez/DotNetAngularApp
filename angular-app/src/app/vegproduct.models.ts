export interface VegProductCreation{
    id?: number;
    name: string;
    price: number;
    description?: string;
    stockQuantity?: number;
    idCategory?: number | null;
}