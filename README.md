# GluhTest

Changed supplier5, supplier6 to have ID 5, ID 6 

        /// The idea is to have few iternations to retrieve data

        My thoughts
        We might have infinite iterations but based on the data of supplier and products we might not be able all the products 
        The idea to first quickly get the bulk data off from the Purchaser requirement with the first few iterations and then individually try to 
        get purchaser data.
        Believe an unit test would have added much as i didnt modularise the code too much and could test the output.

        /// 1) First iterate - bulk and in stock
        /// 2) Second iterate - bulk and in stock but no minimum quantity for Suppliers from 1st iterate
        /// 3) Third iterate -  bulk and in stock but with allstock of Supplier but not completely fullfiled
        /// 4) Fourth iterate - iterate individual product requirement